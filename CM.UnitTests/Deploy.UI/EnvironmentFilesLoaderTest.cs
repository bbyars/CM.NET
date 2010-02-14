using System.Collections.Generic;
using CM.Common;
using CM.Deployer;
using Moq;
using NUnit.Framework;

namespace CM.UnitTests.Deploy.UI
{
    [TestFixture]
    public class EnvironmentFilesLoaderTest
    {
        [Test]
        public void ShouldLoadPropertiesFromEnvironmentsFile()
        {
            var stubFileSystem = new Mock<FileSystem>();
            stubFileSystem.Setup(fs => fs.ListAllFilesIn("Environments", "*.properties")).Returns(new[] { "prod.properties" });
            stubFileSystem.Setup(fs => fs.ReadAllText(@"Environments\prod.properties"))
                .Returns(@"<?xml version='1.0' encoding='utf-8'?>
                    <Project xmlns='http://schemas.microsoft.com/developer/msbuild/2003'>
                        <PropertyGroup>
                            <key1>value1</key1>
                            <key2>value2</key2>
                        </PropertyGroup>
                    </Project>");
//            var presenter = new DeployFormPresenter(mockView.Object, stubFileSystem.Object, new ProcessRunner(""), null);
//
//            presenter.LoadEnvironment("prod");
//
//            var expected = new Dictionary<string, string> { { "key1", "value1" }, { "key2", "value2" } };
//            mockView.Verify(v => v.ShowProperties(It.Is<IDictionary<string, string>>(actual => ValueEquals(expected, actual))));
        }
    }
}