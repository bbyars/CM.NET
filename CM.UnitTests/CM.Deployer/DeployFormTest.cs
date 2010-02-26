using System;
using System.Collections.Generic;
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
        public void LoadingFormShouldShowEnvironmentOptions()
        {
            environmentLoader.Setup(loader => loader.GetEnvironments()).Returns(new[] {"dev", "qa", "prod"});
            form.LoadForm(this, EventArgs.Empty);
            Assert.That(form.Environments, Is.EqualTo(new[] {"dev", "qa", "prod"}));
        }

        [Test]
        public void LoadingFormShouldDefaultToSelectingEnvironment()
        {
            form.LoadForm(this, EventArgs.Empty);
            Assert.That(form.UsePackagedEnvironment, Is.True);
            Assert.That(form.EnvironmentEnabled, Is.True);
            Assert.That(form.ExternalFileEnabled, Is.False);
        }

        [Test]
        public void ChangingRadioAllowsUsingExternalFile()
        {
            form.UsePackagedEnvironment = false;
            form.ClickRadio(this, EventArgs.Empty);

            Assert.That(form.UsePackagedEnvironment, Is.False);
            Assert.That(form.EnvironmentEnabled, Is.False);
            Assert.That(form.ExternalFileEnabled, Is.True);
        }

        [Test]
        public void SelectingEnvironmentLoadsProperties()
        {
            var properties = new List<KeyValuePair<string, string>>
                {new KeyValuePair<string, string>("key1", "value1"), new KeyValuePair<string, string>("key2", "value2")};
            environmentLoader.Setup(loader => loader.GetProperties("dev")).Returns(properties);
            form.Environments = new[] {"dev"};
            form.SelectedEnvironment = "dev";

            form.EnvironmentSelected(this, EventArgs.Empty);

            Assert.That(form.Properties, Is.EqualTo(properties));
        }
    }
}