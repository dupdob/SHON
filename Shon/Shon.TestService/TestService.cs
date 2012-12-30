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
        public void Start()
        {
            TestTracer.Log("Start");
            Configuration config =
                ConfigurationManager.OpenExeConfiguration(
                ConfigurationUserLevel.None);
            TestTracer.Log(config.AppSettings.Settings["Demo"].Value);

        }

        public void Stop()
        {
            TestTracer.Log("Stop");
        }

        public void Dispose()
        {
            TestTracer.Log("Dispose");
        }
    }

}
