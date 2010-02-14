using System;
using System.Collections.Generic;
using CM.Common;
using CM.Deployer;
using CM.Deployer.Properties;
using Moq;
using NUnit.Framework;

namespace CM.UnitTests.Deploy.UI
{
    [TestFixture]
    public class DeployFormPresenterTest
    {
        [Test]
        public void ShouldLoadEnvironmentsWhenInitialized()
        {
            var mockView = new Mock<IDeployView>();
            var stubEnvironmentLoader = new Mock<IEnvironmentLoader>();
            stubEnvironmentLoader.Setup(env => env.GetEnvironments()).Returns(new[] {"prod", "qa", "dev"});
            var presenter = new DeployFormPresenter(mockView.Object, new ProcessRunner(""), stubEnvironmentLoader.Object);

            presenter.Initialize();

            mockView.Verify(v => v.ShowEnvironments(new[] {"prod", "qa", "dev"}));
        }

        [Test]
        public void ShouldDisableExternalConfigWhenPackagedEnvironmentSelected()
        {
            var mockView = new Mock<IDeployView>();
            mockView.SetupGet(v => v.UsePackagedEnvironment).Returns(true);
            var presenter = new DeployFormPresenter(mockView.Object, new ProcessRunner(""), null);

            presenter.ToggleConfigSelection();

            mockView.VerifySet(v => v.EnvironmentEnabled = true);
            mockView.VerifySet(v => v.ExternalFileEnabled = false);
        }

        [Test]
        public void ShouldEnableExternalConfigWhenPackagedEnvironmentNotSelected()
        {
            var mockView = new Mock<IDeployView>();
            mockView.SetupGet(v => v.UsePackagedEnvironment).Returns(false);
            var presenter = new DeployFormPresenter(mockView.Object, new ProcessRunner(""), null);

            presenter.ToggleConfigSelection();

            mockView.VerifySet(v => v.EnvironmentEnabled = false);
            mockView.VerifySet(v => v.ExternalFileEnabled = true);
        }

        [Test]
        public void ShouldLoadPropertiesFromEnvironmentFile()
        {
            var mockView = new Mock<IDeployView>();
            var properties = new Dictionary<string, string> { { "key1", "value1" }, { "key2", "value2" } };
            var stubEnvironmentLoader = new Mock<IEnvironmentLoader>();
            stubEnvironmentLoader.Setup(env => env.GetProperties("prod")).Returns(properties);
            var presenter = new DeployFormPresenter(mockView.Object, new ProcessRunner(""), stubEnvironmentLoader.Object);

            presenter.LoadEnvironment("prod");

            mockView.Verify(v => v.ShowProperties(properties));
        }

        [Test]
        public void ShouldRunDeployProcessWithEnvironmentFile()
        {
            var stubView = new Mock<IDeployView>();
            stubView.SetupGet(v => v.UsePackagedEnvironment).Returns(true);
            stubView.SetupGet(v => v.SelectedEnvironment).Returns("prod");
            var stubFileSystem = new Mock<FileSystem>();
            var mockProcessRunner = new Mock<ProcessRunner>("");
            var presenter = new DeployFormPresenter(stubView.Object, mockProcessRunner.Object, null);

            presenter.Deploy();

            var expectedCommand = string.Format(@"C:\Windows\Microsoft.NET\Framework\v3.5\msbuild.exe {0} /t:Deploy /p:""ConfigPath={1}\Environments\prod.properties"" /p:""PackageDirectory=.""",
                Settings.Default.MSBuildFilename, Environment.CurrentDirectory);
            mockProcessRunner.Verify(pr => pr.Start(expectedCommand));
        }

        [Test]
        public void ShouldRunDeployWithExternalConfigFile()
        {
            var stubView = new Mock<IDeployView>();
            stubView.SetupGet(v => v.UsePackagedEnvironment).Returns(false);
            stubView.SetupGet(v => v.ExternalFile).Returns("prod.properties");
            var stubFileSystem = new Mock<FileSystem>();
            var mockProcessRunner = new Mock<ProcessRunner>("");
            var presenter = new DeployFormPresenter(stubView.Object, mockProcessRunner.Object, null);

            presenter.Deploy();

            mockProcessRunner.Verify(pr => pr.Start(It.Is<string>(cmd => cmd.Contains("/p:\"ConfigPath=prod.properties\""))));
        }

        [Test]
        public void ShouldTellTheViewToOpenLogViewOnDeployWithTheDeployProcess()
        {
            var mockView = new Mock<IDeployView>();
            var stubFileSystem = new Mock<FileSystem>();
            stubFileSystem.Setup(fs => fs.ListAllFilesIn(It.IsAny<string>(), It.IsAny<string>())).Returns(new[] {"prod.properties"});
            var stubProcess = new Mock<SystemProcess>(null);
            var stubProcessRunner = new Mock<ProcessRunner>();
            stubProcessRunner.Setup(pr => pr.Start(It.IsAny<string>())).Returns(stubProcess.Object);
            var presenter = new DeployFormPresenter(mockView.Object, stubProcessRunner.Object, null);

            presenter.Deploy();

            mockView.Verify(v => v.ShowLogView(stubProcess.Object));
        }
    }
}