using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.Remoting.Lifetime;
using System.Threading;
namespace Shon
{
    /// <summary>
    /// Wrapper used to command payload
    /// </summary>
    internal sealed class PayloadWrapper : MarshalByRefObject, iPayloadWrapper
    {
        #region attributes
        private object _payload;
        private object _synchro = new object();
        private const string startName = "Start";
        private bool _hasCrashed = false;
        private List<Tuple<LogLevel, string>> _messages = new List<Tuple<LogLevel, string>>();
        #endregion
        #region properties
        /// <summary>
        /// 
        /// </summary>
        public bool HasCrashed
        {
            get
            {
                return _hasCrashed;
            }
        }
        #endregion // attributes
        #region methods
        public PayloadWrapper()
        {
            // ensure object is not collected
            ILease lease = (ILease)InitializeLifetimeService();
            lease.InitialLeaseTime = Timeout.InfiniteTimeSpan;
        }

        /// <summary>
        /// Initialize wrapper
        /// </summary>
        /// <param name="assemblyName">assembly containing the payload</param>
        /// <param name="className">class implementing service</param>
        /// <returns></returns>
        public bool Initialize(string assemblyName, string className)
        {
            Log(LogLevel.Debug, string.Format(CultureInfo.InvariantCulture, "Payload creates guest instance: {0} from {1}.", className, assemblyName));
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            // instantiate guest object
            _payload = AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(assemblyName, className);
            if (_payload == null)
            {
                Log(LogLevel.Error, string.Format(CultureInfo.InvariantCulture, "Payload failed to create guest instance: {0} from {1}.", className, assemblyName));
                return false;
            }
            return true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string message;
            Exception ex = e.ExceptionObject as Exception;
            if (ex == null)
            {
                message = "unmanaged exception";
                if (e.ExceptionObject != null)
                {
                    message += " "+e.ExceptionObject.ToString();
                }
            }
            else
            {
                message = string.Format(CultureInfo.InvariantCulture, "{0} '{1}', {2}", ex.GetType().Name, ex.Message, ex.StackTrace);
            }
            Log(LogLevel.Fatal, string.Format(CultureInfo.InvariantCulture, "Guest unhandled exception: {0}.", message));
        }

        private void Log(LogLevel logLevel, string message)
        {
            /// TODO: find a better way to send this information back to master app domain
            lock (_synchro)
            {
                _messages.Add(new Tuple<LogLevel, string>(logLevel, message));
            }
        }

        /// <summary>
        /// Start the service
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void Start(string parameter)
        {
            MethodInfo withParam = _payload.GetType().GetMethod(startName, BindingFlags.Public | BindingFlags.Instance, null, new Type[] { "".GetType() }, null);
            MethodInfo withoutParam = _payload.GetType().GetMethod(startName, BindingFlags.Public | BindingFlags.Instance, null, new Type[] {}, null);
            if (withoutParam != null)
            {
                Log(LogLevel.Trace, "Guest implements Start()");
            }
            if (withParam != null)
            {
                Log(LogLevel.Trace, "Guest implements Start(string parameter)");
            }
            try
            {
                if (parameter != null)
                {
                    if (withParam != null)
                    {
                        Log(LogLevel.Debug, string.Format(CultureInfo.InvariantCulture, "Host calling {0}.Start(\"{1}\")",
                            _payload.GetType(), parameter ));
                        withParam.Invoke(_payload, new object[] { parameter });
                    }
                    else
                    {
                        Log(LogLevel.Fatal, string.Format(CultureInfo.InvariantCulture, "Guest {0} does not implement Start(string parameter), start failed",
                            _payload.GetType()));
                    }
                }
                else
                {
                    Log(LogLevel.Debug, string.Format(CultureInfo.InvariantCulture, "Host calling {0}.Start()",
                       _payload.GetType()));
                    withoutParam.Invoke(_payload, null);
                }
            }
            catch (Exception)
            {
                _hasCrashed = true;
            }
        }

         /// <summary>
        /// Stops the instance
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void Stop()
        {
            if (_hasCrashed)
            {
                return;
            }
            try
            {
                Log(LogLevel.Trace, "Guest implements Stop()");
                InvokePayload("Stop");
            }
            catch (Exception)
            {
                _hasCrashed = true;
            }
        }

        // helper method, call payload method
        private void InvokePayload(string method)
        {
            _payload.GetType().InvokeMember(method,
                BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null,
                _payload, null, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Dispose this intance
        /// </summary>
        public void Dispose()
        {
            if (_payload == null)
            {
                return;
            }
            Type disposInterface = _payload.GetType().GetInterface(typeof(IDisposable).FullName);
            if (disposInterface != null)
            {
                // the payload implements disposable
                disposInterface.InvokeMember("Dispose",
                    BindingFlags.InvokeMethod| BindingFlags.Public|BindingFlags.Instance, null,
                    _payload, null, CultureInfo.InvariantCulture);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public List<Tuple<LogLevel, string>> PopMessage()
        {
            lock (_synchro)
            {
                List<Tuple<LogLevel, string>> result = _messages;
                _messages = new List<Tuple<LogLevel, string>>();
                return result;
            }
        }
        #endregion
    }
}
