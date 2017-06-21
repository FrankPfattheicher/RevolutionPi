using System.IO;
using System.Reflection;
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
            var json = ResourceLoader.LoadAsString(Assembly.GetAssembly(GetType()),
                $"IctBaden.RevolutionPi.Test.{ConfigFileName}");
            File.WriteAllText(ConfigFileName, json);

            configuration = new PiConfiguration
            {
                RevPiConfigFileName = ConfigFileName
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
            Assert.AreEqual(1, configuration.Devices.Length);
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
