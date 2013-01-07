using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;

namespace Shon.TestService
{
    /// <summary>
    /// Service mock
    /// </summary>
    public class TestService: IDisposable
    {
        private string _parameter;
        public void Start()
        {
            TestTracer.Log("Start");
            Configuration config =
                ConfigurationManager.OpenExeConfiguration(
                ConfigurationUserLevel.None);
            TestTracer.Log(config.AppSettings.Settings["Demo"].Value);

        }

        public void Start(string param)
        {
            TestTracer.Log("StartWithParam");
            Configuration config =
                ConfigurationManager.OpenExeConfiguration(
                ConfigurationUserLevel.None);
            TestTracer.Log(config.AppSettings.Settings["Demo"].Value);
            _parameter = param;

            if (_parameter == "SyncRaiseOnStart")
            {
                TestTracer.Log("RaisedException");
                throw new ApplicationException("Raised on start");
            }
        }

        public void Stop()
        {
            TestTracer.Log("Stop");
            if (_parameter == "SyncRaiseOnStop")
            {
                TestTracer.Log("RaisedException");
                throw new ApplicationException("Raised on stop");
            }
        }

        public void Dispose()
        {
            TestTracer.Log("Dispose");
        }
    }

}
