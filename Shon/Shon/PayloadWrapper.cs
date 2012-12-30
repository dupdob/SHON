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
#endregion

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
            // instantiate guest object
            _payload = AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(assemblyName, className);
            return true;
        }

        /// <summary>
        /// Start the service
        /// </summary>
        public void Start()
        {
            InvokePayload("Start");
        }

        /// <summary>
        /// Stops the instance
        /// </summary>
        public void Stop()
        {
            InvokePayload("Stop");
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
