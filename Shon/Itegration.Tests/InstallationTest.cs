using NUnit.Framework;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;

namespace Shon.Test
{
    [TestFixture]
    public class InstallationTest
    {
        [OneTimeSetUp]
        public void Install()
        {
            ServiceController controler;
            string file = typeof(Shon.Host).Assembly.Location;
            if (file.Contains(" "))
            {
                file = '"' + file + '"';
            }
            ProcessStartInfo start = new ProcessStartInfo(file, "/install");
            start.UseShellExecute = true;
            start.Verb = "runas";
            using (Process process = Process.Start(start))
            {
                // run install
            }
            // sleep to ensure status is propagated
            Stopwatch watch = new Stopwatch();
            watch.Start();
            while (watch.ElapsedMilliseconds < 1000)
            {
                controler = new ServiceController("toto");
                Thread.Sleep(50);
                controler.Refresh();
                try
                {
                    if (ServiceControllerStatus.Stopped == controler.Status)
                    {
                        break;
                    }
                    if (ServiceControllerStatus.Running == controler.Status)
                    {
                        controler.Stop();
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
            controler = new ServiceController("toto");
            Assert.AreEqual(ServiceControllerStatus.Stopped, controler.Status);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
           ServiceController controler;
            controler = new ServiceController("toto");
            try
            {
            }
            finally
            {
                string file = typeof(Shon.Host).Assembly.Location;
                ProcessStartInfo start = new ProcessStartInfo(@"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe", "/uninstall "+file);
                start.UseShellExecute = true;
                start.Verb = "runas";
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
            var controler = new ServiceController("toto");
            if (controler.Status == ServiceControllerStatus.Running)
            {
                controler.Stop();
                Thread.Sleep(2000);
            }
            controler.Start();
            var watch = new Stopwatch();
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
