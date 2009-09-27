using System;
using System.Collections.Generic;
using CM.Common;
using CM.Deploy.UI;
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
            var stubFileSystem = new Mock<FileSystem>();
            stubFileSystem.Setup(fs => fs.ListAllFilesIn("Environments", "*.properties"))
                .Returns(new[] {"prod.properties", "qa.properties", "dev.properties"});
            var presenter = new DeployFormPresenter(mockView.Object, stubFileSystem.Object, new ProcessRunner(""));

            presenter.Initialize();

            mockView.Verify(v => v.ShowEnvironments(new[] {"prod", "qa", "dev"}));
        }

        [Test]
        public void ShouldDisableExternalConfigWhenPackagedEnvironmentSelected()
        {
            var mockView = new Mock<IDeployView>();
            mockView.SetupGet(v => v.UsePackagedEnvironment).Returns(true);
            var stubFileSystem = new Mock<FileSystem>();
            var presenter = new DeployFormPresenter(mockView.Object, stubFileSystem.Object, new ProcessRunner(""));

            presenter.ToggleConfigSelection();

            mockView.VerifySet(v => v.EnvironmentEnabled = true);
            mockView.VerifySet(v => v.ExternalFileEnabled = false);
        }

        [Test]
        public void ShouldEnableExternalConfigWhenPackagedEnvironmentNotSelected()
        {
            var mockView = new Mock<IDeployView>();
            mockView.SetupGet(v => v.UsePackagedEnvironment).Returns(false);
            var stubFileSystem = new Mock<FileSystem>();
            var presenter = new DeployFormPresenter(mockView.Object, stubFileSystem.Object, new ProcessRunner(""));

            presenter.ToggleConfigSelection();

            mockView.VerifySet(v => v.EnvironmentEnabled = false);
            mockView.VerifySet(v => v.ExternalFileEnabled = true);
        }

        [Test]
        public void ShouldLoadPropertiesFromEnvironmentFile()
        {
            var mockView = new Mock<IDeployView>();
            var stubFileSystem = new Mock<FileSystem>();
            stubFileSystem.Setup(fs => fs.ListAllFilesIn("Environments", "*.properties")).Returns(new[] {"prod.properties"});
            stubFileSystem.Setup(fs => fs.ReadAllText(@"Environments\prod.properties"))
                .Returns(@"<?xml version='1.0' encoding='utf-8'?>
                    <Project xmlns='http://schemas.microsoft.com/developer/msbuild/2003'>
                        <PropertyGroup>
                            <key1>value1</key1>
                            <key2>value2</key2>
                        </PropertyGroup>
                    </Project>");
            var presenter = new DeployFormPresenter(mockView.Object, stubFileSystem.Object, new ProcessRunner(""));

            presenter.LoadEnvironment("prod");

            var expected = new Dictionary<string, string> {{"key1", "value1"}, {"key2", "value2"}};
            mockView.Verify(v => v.ShowProperties(It.Is<IDictionary<string, string>>(actual => ValueEquals(expected, actual))));
        }

        [Test]
        public void ShouldRunDeployProcessWithEnvironmentFile()
        {
            var stubView = new Mock<IDeployView>();
            stubView.SetupGet(v => v.UsePackagedEnvironment).Returns(true);
            stubView.SetupGet(v => v.SelectedEnvironment).Returns("prod");
            var stubFileSystem = new Mock<FileSystem>();
            stubFileSystem.Setup(fs => fs.ListAllFilesIn(".", "*.proj")).Returns(new[] {"build.proj"});
            var mockProcessRunner = new Mock<ProcessRunner>("");
            var presenter = new DeployFormPresenter(stubView.Object, stubFileSystem.Object, mockProcessRunner.Object);

            presenter.Deploy();

            const string expectedCommand = @"build.proj /t:Deploy /p:""ConfigPath=Environments\prod.properties"" /p:""PackageDirectory=.""";
            mockProcessRunner.Verify(pr => pr.Run(expectedCommand, TimeSpan.MaxValue));
        }

        [Test]
        public void ShouldRunDeployWithExternalConfigFile()
        {
            var stubView = new Mock<IDeployView>();
            stubView.SetupGet(v => v.UsePackagedEnvironment).Returns(false);
            stubView.SetupGet(v => v.ExternalFile).Returns("prod.properties");
            var stubFileSystem = new Mock<FileSystem>();
            stubFileSystem.Setup(fs => fs.ListAllFilesIn(".", "*.proj")).Returns(new[] { "build.proj" });
            var mockProcessRunner = new Mock<ProcessRunner>("");
            var presenter = new DeployFormPresenter(stubView.Object, stubFileSystem.Object, mockProcessRunner.Object);

            presenter.Deploy();

            const string properties = "build.proj /t:Deploy /p:\"ConfigPath=prod.properties\" /p:\"PackageDirectory=.\"";
            mockProcessRunner.Verify(pr => pr.Run(properties, TimeSpan.MaxValue));
        }

        [Test]
        public void ShouldTellTheViewToOpenLogViewOnDeploy()
        {
            var mockView = new Mock<IDeployView>();
            var stubFileSystem = new Mock<FileSystem>();
            stubFileSystem.Setup(fs => fs.ListAllFilesIn(It.IsAny<string>(), It.IsAny<string>())).Returns(new[] {"prod.properties"});
            var stubProcessRunner = new Mock<ProcessRunner>("");
            var presenter = new DeployFormPresenter(mockView.Object, stubFileSystem.Object, stubProcessRunner.Object);

            presenter.Deploy();

            mockView.Verify(v => v.ShowLogView(stubProcessRunner.Object, It.IsAny<string[]>()));
        }

        private static bool ValueEquals(IDictionary<string, string> expected, IDictionary<string, string> actual)
        {
            if (expected.Keys.Count != actual.Keys.Count)
                return false;

            foreach (var key in expected.Keys)
            {
                if (!actual.ContainsKey(key))
                    return false;
                if (!actual[key].Equals(expected[key]))
                    return false;
            }

            return true;
        }
    }
}