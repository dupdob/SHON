using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace Shon
{
    /// <summary>
    /// Manages the payload component, initialization and command forwarding
    /// </summary>
    public class Host: IDisposable
    {
        #region attributes
        private AppDomain _domain;
        private iPayloadWrapper _payload;
        private PayloadDescription _description;
        #endregion

        #region properties
        /// <summary>
        /// Appdomain of the paylod
        /// </summary>
		public AppDomain Domain
        {
            get { return _domain; }
        }
 
	    #endregion

        #region methods
        /// <summary>
        /// Initialze payload component
        /// </summary>
        /// <param name="description">payload description</param>
        /// <returns>true if successful</returns>               
        public bool Initialize(PayloadDescription description)
        {
            AppDomainSetup setup = new AppDomainSetup();
            if (description == null)
            {
                throw new ArgumentNullException("description");
            }
            _description = description;
            // work the creation of the appomain
            setup.ApplicationBase = description.BinaryFolder;
            setup.ConfigurationFile = description.ConfigurationFile;
            _domain=AppDomain.CreateDomain(string.Format(CultureInfo.CurrentCulture, "{0}.{1}", description.Assembly, description.Class), null, setup);
            // creates payload wrapper in charge of interaction with guest
            _payload = (iPayloadWrapper)_domain.CreateInstanceFromAndUnwrap(typeof(PayloadWrapper).Assembly.CodeBase, typeof(PayloadWrapper).FullName);
            _payload.Initialize(description.Assembly, _description.Class);
            return true;
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~Host()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose the instance
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose implementation
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_payload != null)
                {
                    // dispose payloadwrapper
                    _payload.Dispose();
                    // unload appdomain
                    AppDomain.Unload(_domain);
                }
            }
        }

        /// <summary>
        /// Start the service
        /// </summary>
        public void Start()
        {
            _payload.Start(_description.Parameter);
        }

        /// <summary>
        /// Stop the service
        /// </summary>
        public void Stop()
        {
            _payload.Stop();
        }
        #endregion
    }
}
