using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shon;
using System.IO;

namespace Shon.Test
{
    [TestClass]
    public class RedirectionTest
    {
        [TestMethod]
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

        [TestMethod]
        public void ErrorFailureTest()
        {
            string filename = "invalidname?/";
            using (ErrorRedirection direct = new ErrorRedirection())
            {
                Assert.IsFalse(direct.Init(filename), "Init must fail if filename is invalid");
            }
        }
    }
}
