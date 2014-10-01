using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
namespace Shon
{
    /// <summary>
    /// Wrapper class for PInovke
    /// </summary>
    internal static class NativeMethods
    {
        [DllImport("kernel32")] [return:MarshalAs(UnmanagedType.Bool)]
        static extern public  Boolean SetStdHandle(int stdHandleId, IntPtr handle);

        [DllImport("kernel32")]
        static extern public IntPtr GetStdHandle(int stdHandledId);
    }
    /// <summary>
    /// Handles low level redirection of the standard error output
    /// </summary>
    public sealed class ErrorRedirection: IDisposable
    {
        #region attributes
        private AnonymousPipeServerStream _server;
        private AnonymousPipeClientStream _client;
        private StreamWriter _errorWriter;
        private FileStream _logFile;
        private TextWriter _previousErrorWriter;
        private TextWriter _outputWriter;
        private IntPtr ? _originalErrorHandle;
        private readonly byte[] _buffer;
        private readonly object _synchro = new object();
        private bool _closed = false;
        private bool _closing = false;
        // default code page, could be made configurable at a later stage
        private readonly int _codepage = 1251;
        #endregion // attributes

        #region methods
        /// <summary>
        /// Default constructor
        /// </summary>
        public ErrorRedirection()
        {
            _server = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.None, 512);
            _client = new AnonymousPipeClientStream(PipeDirection.Out, _server.ClientSafePipeHandle);
            _buffer = new byte[512];
        }
        
        /// <summary>
        /// Finalizer
        /// </summary>
        ~ErrorRedirection()
        {
            Dispose(false);
        }

        /// <summary>
        /// Initialize redirection
        /// </summary>
        /// <returns>true if successful</returns>
        // FXCop warning addressed
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Runtime.InteropServices.SafeHandle.DangerousGetHandle")]
        public bool Init(string fileName)
        {
            try
            {
                // init error redirection
                // initalize server read
                // logfile
                // open file
                _logFile = new FileStream(fileName, FileMode.OpenOrCreate);
                // output writer
                _outputWriter = new StreamWriter(_logFile);
                ListenForData();
                // create writer for .Net error
                _errorWriter = new StreamWriter(_client, System.Text.Encoding.GetEncoding(_codepage));
                // .net error redirection
                _previousErrorWriter = Console.Error;
                Console.SetError(_errorWriter);
                // native error
                _originalErrorHandle= NativeMethods.GetStdHandle(-12);
                // scaffolding for handle management
                bool mustReleaseSafeHandle = false;
                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                }
                finally
                {
                    SafePipeHandle handle = _client.SafePipeHandle;
                    handle.DangerousAddRef(ref mustReleaseSafeHandle);
                    NativeMethods.SetStdHandle(-12, handle.DangerousGetHandle());
                    if (mustReleaseSafeHandle)
                        handle.DangerousRelease();
                }
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch (IOException)
            {
                return false;
            }
            return true;
        }

        //handle writing
        private void ErrorText(IAsyncResult result)
        {
            int dataCount=_server.EndRead(result);
            if (dataCount > 0)
            {
                string message = System.Text.Encoding.GetEncoding(_codepage).GetString(_buffer, 0, dataCount);
                RedirectError(message);
                ListenForData();
            }
            else
            {
                // server closed
                lock (_synchro)
                {
                    _closed = true;
                    _outputWriter.Dispose();
                    // close the server pipe
                    _server.Dispose();
                    // close the file
                    _logFile.Dispose();
                    // notify we're done
                    Monitor.Pulse(_synchro);
                }
            }
        }

        private void RedirectError(string message)
        {
            _outputWriter.Write(message);
        }

        private void ListenForData()
        {
            _server.BeginRead(_buffer, 0, _buffer.GetLength(0), ErrorText, null);
        }
        /// <summary>
        /// Dispose this instance
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_server"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_logFile"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_outputWriter")]
        private void Dispose(bool disposing)
        {
            // restore
            if (_originalErrorHandle.HasValue)
            {
                NativeMethods.SetStdHandle(-12, _originalErrorHandle.Value);
                _originalErrorHandle = null;
            }
            if (_previousErrorWriter != null)
            {
                Console.SetError(_previousErrorWriter);
                _previousErrorWriter = null;
            }
            // dispose resource
            if (disposing)
            {
               if (_errorWriter != null)
                {
                    _errorWriter.Dispose();
                    _errorWriter = null;
                }
                // stop the windows error redirection pipes
                lock (_synchro)
                {
                    if (!_closed)
                    {
                        if (!_closing)
                        {
                            _closing = true;
                            // close the client pipe to drop redirection
                            _client.Dispose();
                        }
                        // wait for server part dispose
                        if (Monitor.Wait(this._synchro, 5000))
                        {
                            _server = null;
                        }
                    }
                }
            }
        }
        
        #endregion
    }
}
