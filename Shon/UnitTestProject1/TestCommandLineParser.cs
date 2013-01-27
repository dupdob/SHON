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
        }

        [Test]
        public void ParseOptionsWithValues()
        {
            var parser = new CommandLineParser();
            parser.RegisterParamValue("i");
            parser.Parse(new string[] { "file.txt", "/i", "fileinputname" });
            Assert.AreEqual(2, parser.ParametersCount, "Invalid count of parameters");

        }
    }
}
