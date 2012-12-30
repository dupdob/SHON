using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using Shon.TestService;
namespace Shon.Test
{
    [TestClass]
    public class HostTest
    {

        [TestMethod]
        public void BasicHostTest()
        {
            using (Host test = new Host())
            {
                PayloadDescription desc = new PayloadDescription();
                desc.Assembly = Assembly.GetExecutingAssembly().Location;
                desc.Class = typeof(HostTest).FullName;
                Assert.IsTrue(test.Initialize(desc), "Initialize must succeed");
            }
        }

        [TestMethod]
        public void InitTest()
        {
            using (Host test = new Host())
            {
                PayloadDescription desc = new PayloadDescription();
                desc.Assembly = typeof(TestService.TestService).Assembly.Location;
                desc.Class = typeof(TestService.TestService).FullName;
                Assert.IsTrue(test.Initialize(desc), "Initialize must succeed");
                using (TestTracer tracer = TestTracer.FromDomain(test.Domain))
                {
                    test.Start();

                    Assert.AreEqual("Start", tracer.Pop(), "TestMethod.Start should have been called");

                    Assert.AreEqual("Default", tracer.Pop(), "Bad test parameter");
                    test.Stop();

                    Assert.AreEqual("Stop", tracer.Pop(), "TestMethod.Stop should have been called");

                }
            }

        }

        [TestMethod]
        public void AlternateTest()
        {
            using (Host test = new Host())
            {
                PayloadDescription desc = new PayloadDescription();
                desc.Assembly = typeof(TestService.TestService).Assembly.Location;
                desc.Class = typeof(TestService.TestService).FullName;
                desc.ConfigurationFile = "AltShon.TestService.config";
                Assert.IsTrue(test.Initialize(desc), "Initialize must succeed");
                using (TestTracer tracer = TestTracer.FromDomain(test.Domain))
                {
                    test.Start();

                    Assert.AreEqual("Start", tracer.Pop(), "TestMethod.Start should have been called");

                    Assert.AreEqual("Alt", tracer.Pop(), "Bad test parameter");
                    test.Stop();

                    Assert.AreEqual("Stop", tracer.Pop(), "TestMethod.Stop should have been called");

                }
            }
        }
//        [TestMethod]
        public void FinalizerTest()
        {
            Host test = new Host();
            PayloadDescription desc = new PayloadDescription();
            desc.Assembly = typeof(TestService.TestService).Assembly.Location;
            desc.Class = typeof(TestService.TestService).FullName;
            desc.ConfigurationFile = "AltShon.TestService.config";
            Assert.IsTrue(test.Initialize(desc), "Initialize must succeed");
            test = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HostMethodChecks()
        {
            using (Host test = new Host())
            {
                test.Initialize(null);
            }
        }
    }
}
