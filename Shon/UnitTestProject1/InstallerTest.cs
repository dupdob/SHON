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
            if (!ShonInstaller.HasServiceInstallationRigths())
                Assert.Ignore("Need admin rigthts for this test");
            try
            { 
                ShonInstaller.InstallService("toto");

                // sleep to ensure status is propagated
                var watch = new Stopwatch();
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
                ShonInstaller.UninstallService("toto");
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
