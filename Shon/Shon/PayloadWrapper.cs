using System;
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
        private const string startName = "Start";
        private bool _hasCrashed = false;
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
        /// <summary>
        /// Raised for logging
        /// </summary>
        public event LogHandler Logging;
        #endregion // attributes
        #region methods
        public PayloadWrapper()
        {
            // ensure object is not collected
            ILease lease = (ILease)InitializeLifetimeService();
            lease.InitialLeaseTime = Timeout.InfiniteTimeSpan;
        }

        private void Log(LogLevel level, string message)
        {
            if (Logging != null)
            {
                Logging(level, message);
            }
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
            }
            return true;
        }

        /// <summary>
        /// Start the service
        /// </summary>
        public void Start(string parameter)
        {
            MethodInfo withParam = _payload.GetType().GetMethod(startName, BindingFlags.Public | BindingFlags.Instance, null, new Type[] { "".GetType() }, null);
            MethodInfo withoutParam = _payload.GetType().GetMethod(startName, BindingFlags.Public | BindingFlags.Instance, null, new Type[] {}, null);
            try
            {
                if (parameter != null)
                {
                    Log(LogLevel.Trace, "Guest implements Start(string parameter).");
                    if (withParam != null)
                    {
                        withParam.Invoke(_payload, new object[] { parameter });
                    }
                    else
                    {
                    }
                }
                else
                {
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
            Type disposInterface = _payload.GetType().GetInterface(typeof(IDisposable).FullName);
            if (disposInterface != null)
            {
                // the payload implements disposable
                disposInterface.InvokeMember("Dispose",
                    BindingFlags.InvokeMethod| BindingFlags.Public|BindingFlags.Instance, null,
                    _payload, null, CultureInfo.InvariantCulture);
            }
        }
	    #endregion
    }
}
