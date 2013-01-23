using NUnit.Framework;
using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;

namespace Shon.Test
{
    [TestFixture]
    public class InstallerTest
    {
        [TestFixtureSetUp]
        public void Install()
        {
            ServiceController controler;
            ProcessStartInfo start = new ProcessStartInfo(@"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe", "shon.exe");
            start.UseShellExecute = false;
            using (Process process = Process.Start(start))
            {
                // run install
            }
            // sleep to ensure status is propagated
            Stopwatch watch = new Stopwatch();
            watch.Start();
            while (watch.ElapsedMilliseconds < 1000)
            {
                Thread.Sleep(50);
                controler = new ServiceController("toto");
                try
                {
                    if (ServiceControllerStatus.Stopped == controler.Status)
                    {
                        break;
                    }
                }
                catch
                {
                    // exception is raised if service not ready
                }
                finally
                {
                    controler.Dispose();
                }
            }
        }
        [TestFixtureTearDown]
        public void TearDown()
        {
           ServiceController controler;
            controler = new ServiceController("toto");
            try
            {
            }
            finally
            {
                ProcessStartInfo start = new ProcessStartInfo(@"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe", "/uninstall shon.exe");
                start.UseShellExecute = false;
                using (Process process = Process.Start(start))
                {
                    // run install
                }
            }
            Thread.Sleep(100);
            try
            {
                Assert.AreEqual(ServiceControllerStatus.Stopped, controler.Status);
                Assert.Fail("Service found,should have been uninstalled");
            }
            catch
            {
            }

        }
        [Test(Description = "Check service starts")]
        public void StartTest()
        {
            ServiceController controler;
            controler = new ServiceController("toto");
            if (controler.Status == ServiceControllerStatus.Running)
            {
                controler.Stop();
                Thread.Sleep(2000);
            }
            controler.Start();
            Stopwatch watch = new Stopwatch();
            watch.Start();
            while (watch.ElapsedMilliseconds < 1000)
            {
                controler.Refresh();
                if (controler.Status == ServiceControllerStatus.Running)
                    break;
                Thread.Sleep(50);
            }
            Assert.AreEqual(ServiceControllerStatus.Running, controler.Status, "Service should be running");
        }
    }
}
