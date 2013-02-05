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
        [Test]
        public void BasicInstallTest()
        {
            ServiceController controler;
            try
            {
                /*string assembly = typeof(Host).Assembly.CodeBase;
                ProcessStartInfo start = new ProcessStartInfo(@"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe", assembly);
                start.UseShellExecute = false;
                using (Process process = Process.Start(start))
                {
                    // run install
                }
                 * */
                ShonInstaller.InstallService();
                // sleep to ensure status is propagated
                Stopwatch watch = new Stopwatch();
                while (watch.ElapsedMilliseconds < 1000)
                {
                    Thread.Sleep(100);
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
                    }
                    finally
                    {
                        controler.Dispose();
                    }
                }
                controler = new ServiceController("toto");
                try
                {
                    Assert.AreEqual(ServiceControllerStatus.Stopped, controler.Status);
                }
                catch
                {
                    Assert.Fail("Service not found, installation failed");
                }
                finally
                {
                    controler.Dispose();
                }
            }
            finally
            {
                 string file = typeof(Shon.Host).Assembly.Location;
                if (file.Contains(" "))
                {
                    file = '"' + file + '"';
                }
              ProcessStartInfo start = new ProcessStartInfo(@"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe", "/uninstall "+file);
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
    }
}
