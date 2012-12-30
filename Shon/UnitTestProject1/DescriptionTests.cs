using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Shon.Test
{
    [TestClass]
    public class DescriptionTests
    {
        [TestMethod]
        public void AutoConf()
        {
            PayloadDescription desc = new PayloadDescription();
            string assembly=Assembly.GetExecutingAssembly().Location;
            desc.Assembly = Path.GetFileName(assembly);
            desc.Class = typeof(HostTest).FullName;
            Assert.AreEqual(assembly+".config", desc.ConfigurationFile, "Config file should match default name");
            Assert.AreEqual(Path.GetDirectoryName(assembly), desc.BinaryFolder, "binary folder should be assembly folder");
        }
        [TestMethod]
        public void ConfSerialization()
        {
            string filename = "testconfig.xml";
            try
            {
                PayloadDescription desc = new PayloadDescription();
                string assembly = Assembly.GetExecutingAssembly().Location;
                desc.Assembly = assembly;
                desc.Class = typeof(HostTest).FullName;
                File.Delete(filename);
                PayloadDescription.Save(desc, filename);

                Assert.IsTrue(File.Exists(filename));
            }
            finally
            {
                File.Delete(filename);
            }
        }

        [TestMethod]
        public void ConfLoad()
        {
            string filename = "testfile.xml";
            PayloadDescription desc = new PayloadDescription();
            string assembly = Assembly.GetExecutingAssembly().Location;
            desc.Assembly = assembly;
            desc.Class = typeof(HostTest).FullName;
            desc=PayloadDescription.Load(filename);

            Assert.IsNotNull(desc, "Failed to deserialize config");
        }

    }
}
