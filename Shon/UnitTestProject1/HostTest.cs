using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Shon.TestService;
using NUnit.Framework;
using System.IO;
using System.Threading;
namespace Shon.Test
{
    [TestFixture]
    public class HostTest
    {
        [Test]
        //basic smoke test
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

        [Test]
        // perform a real initialization of the host
        public void InitTest()
        {
            using (Host test = new Host())
            {
                PayloadDescription desc = new PayloadDescription();
                string assembly = typeof(TestService.TestService).Assembly.CodeBase;
                desc.Assembly = assembly;
                desc.Class = typeof(TestService.TestService).FullName;
                Assert.IsTrue(test.Initialize(desc), "Initialize must succeed");
                using (TestTracer tracer = TestTracer.FromDomain(test.Domain))
                {
                    test.Start();

                    Assert.AreEqual("Start", tracer.Pop(), "TestMethod.Start should have been called");

                    Assert.AreEqual("Default", tracer.Pop(), "Bad test parameter");
                    test.Stop();

                    Assert.AreEqual("Stop", tracer.Pop(), "TestMethod.Stop should have been called");

                    tracer.AssertEmpty();
                }
            }

        }

        [Test]
        // perform a real initialization of the host
        public void CrashOnStartTest()
        {
            using (Host test = new Host())
            {
                PayloadDescription desc = new PayloadDescription();
                string assembly = typeof(TestService.TestService).Assembly.CodeBase;
                desc.Assembly = assembly;
                desc.Class = typeof(TestService.TestService).FullName;
                desc.Parameter = "SyncRaiseOnStart";
                Assert.IsTrue(test.Initialize(desc), "Initialize must succeed");
                using (TestTracer tracer = TestTracer.FromDomain(test.Domain))
                {
                    test.Start();

                    Assert.AreEqual("StartWithParam", tracer.Pop(), "TestMethod.Start should have been called");
                    Assert.AreEqual("Default", tracer.Pop(), "Bad test parameter");

                    Assert.AreEqual("RaisedException", tracer.Pop(), "Should have raised an exception");
                    tracer.AssertEmpty();
                }
            }
        }

        [Test]
        // perform a real initialization of the host
        public void CrashOnStopTest()
        {
            using (Host test = new Host())
            {
                PayloadDescription desc = new PayloadDescription();
                string assembly = typeof(TestService.TestService).Assembly.CodeBase;
                desc.Assembly = assembly;
                desc.Class = typeof(TestService.TestService).FullName;
                desc.Parameter = "SyncRaiseOnStop";
                Assert.IsTrue(test.Initialize(desc), "Initialize must succeed");
                using (TestTracer tracer = TestTracer.FromDomain(test.Domain))
                {
                    test.Start();

                    Assert.AreEqual("StartWithParam", tracer.Pop(), "TestMethod.Start should have been called");
                    Assert.AreEqual("Default", tracer.Pop(), "Bad test parameter");
                    test.Stop();
                    Assert.AreEqual("Stop", tracer.Pop(), "Should have raised an exception");
                    Assert.AreEqual("RaisedException", tracer.Pop(), "Should have raised an exception");
                    tracer.AssertEmpty();
                }
            }
        }

        [Test]
        // test using a guest with a specified configuration file
        public void AlternateTest()
        {
            using (Host test = new Host())
            {
                PayloadDescription desc = new PayloadDescription();
                string assembly = typeof(TestService.TestService).Assembly.CodeBase;
                desc.Assembly = assembly;
                desc.Class = typeof(TestService.TestService).FullName;
                desc.ConfigurationFile =Path.GetFullPath("AltShon.TestService.config");
                Assert.IsTrue(test.Initialize(desc), "Initialize must succeed");
                using (TestTracer tracer = TestTracer.FromDomain(test.Domain))
                {
                    test.Start();

                    Assert.AreEqual("Start", tracer.Pop(), "TestMethod.Start should have been called");

                    Assert.AreEqual("Alt", tracer.Pop(), "Bad test parameter");
                    test.Stop();

                    Assert.AreEqual("Stop", tracer.Pop(), "TestMethod.Stop should have been called");
                    tracer.AssertEmpty();
                }
            }
        }

        [Test]
        [Explicit]
        public void FinalizerTest()
        {
            object synchro = new object();
            bool done = false;
            Host test = new Host();
            PayloadDescription desc = new PayloadDescription();
            desc.Assembly = typeof(TestService.TestService).Assembly.Location;
            desc.Class = typeof(TestService.TestService).FullName;
            desc.ConfigurationFile = "AltShon.TestService.config";
            Assert.IsTrue(test.Initialize(desc), "Initialize must succeed");
            test = null;
            Thread thread = new Thread (() =>
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                lock (synchro)
                {
                    done = true;
                    Monitor.Pulse(synchro);
                }
            });
            thread.Start();
            lock (synchro)
            {
                if (!done)
                {
                    Monitor.Wait(synchro, 1000);
                }
            }
            thread.Interrupt();
        }
        [Test]
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
