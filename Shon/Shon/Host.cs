using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Shon
{
    /// <summary>
    /// Manages the payload component, initialization and command forwarding
    /// </summary>
    public class Host : IDisposable
    {
        #region attributes
        private AppDomain _domain;
        private iPayloadWrapper _payload;
        private PayloadDescription _description;
        private ILog _logger;
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
            _logger = LogManager.GetLogger(string.Format("Host.{0}.{1}", _description.Assembly, _description.Class));
            // work the creation of the appomain
            setup.ApplicationBase = description.BinaryFolder;
            setup.ConfigurationFile = description.ConfigurationFile;
            _domain=AppDomain.CreateDomain(string.Format(CultureInfo.CurrentCulture, "{0}.{1}", description.Assembly, description.Class), null, setup);
            // creates payload wrapper in charge of interaction with guest
            _payload = (iPayloadWrapper)_domain.CreateInstanceFromAndUnwrap(Path.GetFileName(typeof(PayloadWrapper).Assembly.Location), typeof(PayloadWrapper).FullName);
            _payload.Initialize(description.Assembly, _description.Class);
            LogCapture();
            return true;
        }

        private void LogCapture()
        {
            List<Tuple<LogLevel, string>> messages = _payload.PopMessage();
            foreach(Tuple<LogLevel, string> message in messages)
            {
                Log(message.Item1, message.Item2);
            }
        }

        // logging handler
        public void Log(LogLevel level, string message)
        {
            log4net.Core.Level finalLevel=log4net.Core.Level.Off;
            switch (level)
            {
                case LogLevel.Debug:
                    finalLevel = log4net.Core.Level.Debug;
                    break;
                case LogLevel.Trace:
                    finalLevel = log4net.Core.Level.Trace;
                    break;
                case LogLevel.Info:
                    finalLevel = log4net.Core.Level.Info;
                    break;
                case LogLevel.Warn:
                    finalLevel = log4net.Core.Level.Fatal;
                    break;
                case LogLevel.Error:
                    finalLevel = log4net.Core.Level.Error;
                    break;
                case LogLevel.Fatal:
                    finalLevel = log4net.Core.Level.Fatal;
                    break;
            }
            _logger.Logger.Log(typeof(Host), finalLevel, message, null);
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
                    LogCapture();
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
            LogCapture();
        }

        /// <summary>
        /// Stop the service
        /// </summary>
        public void Stop()
        {
            _payload.Stop();
            LogCapture();
        }
        #endregion

    }
}
