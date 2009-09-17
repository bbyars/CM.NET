using System.Collections.Generic;
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
            var presenter = new DeployFormPresenter(mockView.Object, stubFileSystem.Object);

            presenter.Initialize();

            mockView.Verify(v => v.ShowEnvironments(new[] {"prod", "qa", "dev"}));
        }

        [Test]
        public void ShouldDisableExternalConfigWhenPackagedEnvironmentSelected()
        {
            var mockView = new Mock<IDeployView>();
            mockView.SetupGet(v => v.UsePackagedEnvironment).Returns(true);
            var stubFileSystem = new Mock<FileSystem>();
            var presenter = new DeployFormPresenter(mockView.Object, stubFileSystem.Object);

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
            var presenter = new DeployFormPresenter(mockView.Object, stubFileSystem.Object);

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
            var presenter = new DeployFormPresenter(mockView.Object, stubFileSystem.Object);

            presenter.LoadEnvironment("prod");

            var expected = new Dictionary<string, string> {{"key1", "value1"}, {"key2", "value2"}};
            mockView.Verify(v => v.ShowProperties(It.Is<IDictionary<string, string>>(actual => ValueEquals(expected, actual))));
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