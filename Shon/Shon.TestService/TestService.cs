using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;
using System.Threading;

namespace Shon.TestService
{
    /// <summary>
    /// Service mock
    /// </summary>
    public class TestService: IDisposable
    {
        private string _parameter;
        private bool _crashOnRun = false;
        private int _wait = 100;
        private Thread _worker;
        private readonly object _synchro = new object();

        public TestService()
        {
            _worker = new Thread(Run);
        }

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
            if (_parameter == "AsyncRaiseOnRun")
            {
                _crashOnRun = true;
            }
            _worker.Start();
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

        private void Run()
        {
            if (_crashOnRun)
            {
                Thread.Sleep(_wait);
                TestTracer.Log("RaisedException");
                throw new ApplicationException("Raised on run");
            }
        }

        public void Dispose()
        {
            TestTracer.Log("Dispose");
        }
    }

}
