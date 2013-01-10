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
        #region attributes
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
            Log(LogLevel.Debug, string.Format("Payload creates guest instance: {0} from {1}.", className, assemblyName));
            // instantiate guest object
            _payload = AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(assemblyName, className);
            if (_payload == null)
            {
                Log(LogLevel.Error, string.Format("Payload failed to create guest instance: {0} from {1}.", className, assemblyName));
                return false;
            }
            return true;
        }

        private void Log(LogLevel logLevel, string message)
        {
            /// TODO: find a way to send this information back to master app domain
            lock (_synchro)
            {
                _messages.Add(new Tuple<LogLevel, string>(logLevel, message));
            }
        }

        /// <summary>
        /// Start the service
        /// </summary>
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
                        Log(LogLevel.Debug, string.Format("Host calling {0}.Start(\"{1}\")",
                            _payload.GetType(), parameter ));
                        withParam.Invoke(_payload, new object[] { parameter });
                    }
                    else
                    {
                    }
                }
                else
                {
                    Log(LogLevel.Debug, string.Format("Host calling {0}.Start()",
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
