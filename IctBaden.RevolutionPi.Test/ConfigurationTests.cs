using System.IO;
using System.Reflection;
using IctBaden.RevolutionPi.Configuration;
using NUnit.Framework;

namespace IctBaden.RevolutionPi.Test
{
    [TestFixture]
    public class ConfigurationTests
    {
        const string ConfigFileName = "config.json";
        private PiConfiguration configuration;

        [SetUp]
        public void TestSetup()
        {
            var assembly = Assembly.GetAssembly(GetType());
            var json = ResourceLoader.LoadAsString(assembly, $"IctBaden.RevolutionPi.Test.{ConfigFileName}");
            var path = Path.GetDirectoryName(assembly.Location) ?? ".";
            var fullName = Path.Combine(path, ConfigFileName);
            File.WriteAllText(fullName, json);

            configuration = new PiConfiguration
            {
                RevPiConfigFileName = fullName
            };
        }

        [Test]
        public void OpenConfigFileShouldSucceed()
        {
            var opened = configuration.Open();
            Assert.IsTrue(opened);
        }

        [Test]
        public void ConfigShouldHaveDevices()
        {
            configuration.Open();
            Assert.AreEqual(1, configuration.Devices.Count);
        }

        [Test]
        public void ConfigShouldHaveInputs()
        {
            configuration.Open();
            Assert.AreEqual(5, configuration.Devices[0].Inputs.Length);
        }

        [Test]
        public void ConfigShouldHaveOutputs()
        {
            configuration.Open();
            Assert.AreEqual(3, configuration.Devices[0].Outputs.Length);
        }
    }
}
