using System.Collections.Generic;
using CM.Common;
using CM.Deployer;
using Moq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace CM.UnitTests.CM.Deployer
{
    [TestFixture]
    public class EnvironmentFilesLoaderTest
    {
        [Test]
        public void ShouldListFilesInEnvironmentsDirectory()
        {
            var stubFileSystem = new Mock<FileSystem>();
            stubFileSystem.Setup(fs => fs.ListAllFilesIn("env", "*.properties")).Returns(
                new[] {"test.properties", "prod.properties"});
            var loader = new EnvironmentFilesLoader(stubFileSystem.Object, "env", ".properties");

            Assert.That(loader.GetEnvironments(), Is.EqualTo(new[] {"test", "prod"}));
        }

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
            var loader = new EnvironmentFilesLoader(stubFileSystem.Object, "Environments", ".properties");

            Assert.That(loader.GetProperties("prod"), Is.EqualTo(new List<KeyValuePair<string, string>>
            {new KeyValuePair<string, string>("key1", "value1"), new KeyValuePair<string, string>("key2", "value2")}));
        }

        [Test]
        public void ShouldLoadPropertiesFromAllPropertyGroups()
        {
            var stubFileSystem = new Mock<FileSystem>();
            stubFileSystem.Setup(fs => fs.ListAllFilesIn("Environments", "*.properties")).Returns(new[] { "prod.properties" });
            stubFileSystem.Setup(fs => fs.ReadAllText(@"Environments\prod.properties"))
                .Returns(@"<?xml version='1.0' encoding='utf-8'?>
                    <Project xmlns='http://schemas.microsoft.com/developer/msbuild/2003'>
                        <PropertyGroup>
                            <key1>value1</key1>
                        </PropertyGroup>
                        <PropertyGroup>
                            <key2>value2</key2>
                        </PropertyGroup>
                    </Project>");
            var loader = new EnvironmentFilesLoader(stubFileSystem.Object, "Environments", ".properties");

            Assert.That(loader.GetProperties("prod"), Is.EqualTo(new List<KeyValuePair<string, string>>
            { new KeyValuePair<string, string>("key1", "value1"), new KeyValuePair<string, string>("key2", "value2") }));
        }

        [Test]
        public void ShouldIncludeNestedPropertyGroups()
        {
            var stubFileSystem = new Mock<FileSystem>();
            stubFileSystem.Setup(fs => fs.ListAllFilesIn("Environments", "*.properties")).Returns(new[] { "prod.properties" });
            stubFileSystem.Setup(fs => fs.ReadAllText(@"Environments\prod.properties"))
                .Returns(@"<?xml version='1.0' encoding='utf-8'?>
                    <Project xmlns='http://schemas.microsoft.com/developer/msbuild/2003' DefaultTargets='Build'>
                        <Target Name='Build'>
                            <PropertyGroup>
                                <key1>value1</key1>
                            </PropertyGroup>
                        </Target>
                    </Project>");
            var loader = new EnvironmentFilesLoader(stubFileSystem.Object, "Environments", ".properties");

            Assert.That(loader.GetProperties("prod"), Is.EqualTo(new List<KeyValuePair<string, string>>
                { new KeyValuePair<string, string>("key1", "value1")}));
        }

        [Test]
        public void ShouldConvertExternalFileToProperties()
        {
            var stubFileSystem = new Mock<FileSystem>();
            stubFileSystem.Setup(fs => fs.ReadAllText(@"c:\ops\config.properties"))
                .Returns(@"<?xml version='1.0' encoding='utf-8'?>
                    <Project xmlns='http://schemas.microsoft.com/developer/msbuild/2003' DefaultTargets='Build'>
                        <Target Name='Build'>
                            <PropertyGroup>
                                <key1>value1</key1>
                            </PropertyGroup>
                        </Target>
                    </Project>");
            var loader = new EnvironmentFilesLoader(stubFileSystem.Object, "Environments", ".properties");

            Assert.That(loader.LoadProperties(@"c:\ops\config.properties"), Is.EqualTo(new List<KeyValuePair<string, string>>
                { new KeyValuePair<string, string>("key1", "value1") }));
        }

        [Test]
        public void ShouldSaveInMSBuildFormat()
        {
            var mockFileSystem = new Mock<FileSystem>();
            var loader = new EnvironmentFilesLoader(mockFileSystem.Object, "Environments", ".properties");
            var properties = new List<KeyValuePair<string, string>>
                {new KeyValuePair<string, string>("key1", "value1"), new KeyValuePair<string, string>("key2", "value2")};

            loader.SaveProperties(properties, @"c:\ops\config.properties");

            mockFileSystem.Verify(fs => fs.WriteAllText(@"c:\ops\config.properties", @"<?xml version='1.0' encoding='utf-8'?>
<Project xmlns='http://schemas.microsoft.com/developer/msbuild/2003'>
    <PropertyGroup>
        <key1>value1</key1>
        <key2>value2</key2>
    </PropertyGroup>
</Project>"));
        }
    }
}