using System;
using CM.Common;
using CM.Deployer;
using Moq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace CM.UnitTests.CM.Deployer
{
    [TestFixture]
    public class DeployFormTest
    {
        private Mock<IEnvironmentLoader> environmentLoader;
        private Mock<IDeployCommandBuilder> commandBuilder;
        private Mock<ProcessRunner> processRunner;
        private DeployForm form;

        [SetUp]
        public void CreateForm()
        {
            environmentLoader = new Mock<IEnvironmentLoader>();
            commandBuilder = new Mock<IDeployCommandBuilder>();
            processRunner = new Mock<ProcessRunner>();
            form = new DeployForm(environmentLoader.Object, commandBuilder.Object, processRunner.Object);
        }

        [Test]
        public void InitializingShouldShowEnvironmentOptions()
        {
            environmentLoader.Setup(loader => loader.GetEnvironments()).Returns(new[] {"dev", "qa", "prod"});
            form.Initialize();
            Assert.That(form.Environments, Is.EqualTo(new[] {"dev", "qa", "prod"}));
        }

        [Test]
        public void InitializingShouldDefaultToSelectingEnvironment()
        {
            form.Initialize();
            Assert.That(form.UsePackagedEnvironment, Is.True);
            Assert.That(form.EnvironmentEnabled, Is.True);
            Assert.That(form.ExternalFileEnabled, Is.False);
        }

        [Test]
        public void TogglingConfigSelectionAllowsUsingExternalFile()
        {
            form.UsePackagedEnvironment = false;

            form.ToggleConfigSelection();

            Assert.That(form.EnvironmentEnabled, Is.False);
            Assert.That(form.ExternalFileEnabled, Is.True);
        }

        [Test]
        public void SwitchingToExternalFileShouldClearPropertiesIfNoExternalFileSet()
        {
            environmentLoader.Setup(loader => loader.LoadProperties("")).Throws(new Exception("Should not call this"));
            form.UsePackagedEnvironment = false;
            form.Properties = new PropertyList().Add("key1", "value1");

            form.ToggleConfigSelection();

            Assert.That(form.Properties, Is.EqualTo(new PropertyList()));
        }

        [Test]
        public void SwitchingToExternalFileShouldSetExternalFileProperties()
        {
            var properties = new PropertyList().Add("key1", "value1").Add("key2", "value2");
            environmentLoader.Setup(loader => loader.LoadProperties(@"c:\ops\config.properties")).Returns(properties);
            form.LoadExternalFile(@"c:\ops\config.properties");
            form.Properties = new PropertyList();
            form.UsePackagedEnvironment = false;

            form.ToggleConfigSelection();

            Assert.That(form.Properties, Is.EqualTo(properties));
        }

        [Test]
        public void SwitchingToPackagedFileLoadsEnvironmentProperties()
        {
            var properties = new PropertyList().Add("key1", "value1").Add("key2", "value2");
            environmentLoader.Setup(loader => loader.GetProperties("dev")).Returns(properties);
            form.Environments = new[] { "dev" };
            form.SelectedEnvironment = "dev";
            form.UsePackagedEnvironment = true;

            form.ToggleConfigSelection();

            Assert.That(form.Properties, Is.EqualTo(properties));
        }

        [Test]
        public void LoadingEnvironmenPropertiesShowsPropertiesForCurrentEnvironment()
        {
            var properties = new PropertyList().Add("key1", "value1").Add("key2", "value2");
            environmentLoader.Setup(loader => loader.GetProperties("dev")).Returns(properties);
            form.Environments = new[] {"dev"};
            form.SelectedEnvironment = "dev";

            form.LoadEnvironmentProperties();

            Assert.That(form.Properties, Is.EqualTo(properties));
        }

        [Test]
        public void LoadingShouldSetPropertiesAndExternalFile()
        {
            var properties = new PropertyList().Add("key1", "value1").Add("key2", "value2");
            environmentLoader.Setup(loader => loader.LoadProperties(@"c:\ops\config.properties")).Returns(properties);

            form.LoadExternalFile(@"c:\ops\config.properties");

            Assert.That(form.ExternalFile, Is.EqualTo(@"c:\ops\config.properties"));
            Assert.That(form.Properties, Is.EqualTo(properties));
        }

        [Test]
        public void SavingShouldSaveAllProperties()
        {
            form.Properties = new PropertyList().Add("key1", "value1").Add("key2", "value2");
            form.Save(@"c:\ops\config.properties");
            environmentLoader.Verify(loader => loader.SaveProperties(form.Properties, @"c:\ops\config.properties"));
        }
    }
}