using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shon.Test
{
    [TestFixture]
    public class TestCommandLineParser
    {
        [Test]
        public void ParsingTest()
        {
            var parser = new CommandLineParser();
            parser.Parse(new string[] { "file.txt", "/i", "command" });
            Assert.AreEqual(3, parser.ParametersCount, "Invalid count of parameters");
            Assert.IsTrue(parser.IsActive("i"), "Expected option not found");
        }

        [Test]
        public void ParseOptionsWithValues()
        {
            var parser = new CommandLineParser();
            parser.RegisterParamValue("i");
            parser.Parse(new string[] { "file.txt", "/i", "fileinputname" });   
            Assert.AreEqual(2, parser.ParametersCount, "Invalid count of parameters");
            Assert.AreEqual("fileinputname", parser.GetNamedOption("i"));
            Assert.AreEqual(1, parser.Values.Count, "Bad number of anonymous values");
        }

        [Test]
        public void ParseOptionsWithoutValues()
        {
            var parser = new CommandLineParser();
            parser.Parse(new string[] { "file.txt", "/i", "fileinputname" });
            Assert.AreEqual(3, parser.ParametersCount, "Invalid count of parameters");
            Assert.Throws<System.Collections.Generic.KeyNotFoundException>(() => parser.GetNamedOption("i"));
        }
    }
}
