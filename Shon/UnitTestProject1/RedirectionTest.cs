using System;
using Shon;
using System.IO;
using NUnit.Framework;
using System.Threading;

namespace Shon.Test
{
    [TestFixture]
    public class RedirectionTest
    {
        [Test]
        public void ErrorRedirectionBasicTest()
        {
            string filename = "Test.log";
            string testText = "check";
            using (ErrorRedirection direct = new ErrorRedirection())
            {
                Assert.IsTrue(direct.Init(filename), "Initialization should succeed.");
                Console.Error.Write(testText);

                Assert.IsTrue(File.Exists(filename), "Logfile should have been created");
            }
            // check file content   
            string text = File.ReadAllText(filename);
            File.Delete(filename);
            Assert.AreEqual(testText, text, "Logfile does not contain expected text");
        }

        [Test]
        public void ErrorFailureTest()
        {
            string filename = "invalidname?/";
            using (ErrorRedirection direct = new ErrorRedirection())
            {
                Assert.IsFalse(direct.Init(filename), "Init must fail if filename is invalid");
            }
        }

        [Test]
        public void RedirectionFinalizationTest()
        {
            object synchro = new object();
            bool done = false;
            ErrorRedirection test = new ErrorRedirection();
            test = null;
            Thread thread = new Thread(() =>
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
    }
}
