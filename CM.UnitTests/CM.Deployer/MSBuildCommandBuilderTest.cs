using CM.Common;
using CM.Deployer;
using Moq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace CM.UnitTests.CM.Deployer
{
    [TestFixture]
    public class MSBuildCommandBuilderTest
    {
        [Test]
        public void VerifyCommandLine()
        {
            var builder = new MSBuildCommandBuilder(new FileSystem(), @"C:\dir\msbuild.exe", "test.proj", "deploy");
            Assert.That(builder.CommandLine, Is.EqualTo(@"""C:\dir\msbuild.exe"" ""test.proj"" /t:deploy /p:""PackageDirectory=."" /p:""ConfigPath=DeployConfig.properties"""));
        }

        [Test]
        public void ShouldSaveNewFileWithAllEnvironmentProperties()
        {
            var mockFileSystem = new Mock<FileSystem>();
            var builder = new MSBuildCommandBuilder(mockFileSystem.Object, @"C:\dir\msbuild.exe", "test.proj", "deploy");

            builder.SetEnvironmentProperties(new PropertyList().Add("key1", "value1").Add("key2", "value2"));

            mockFileSystem.Verify(fs => fs.WriteAllText("DeployConfig.properties", @"<?xml version='1.0' encoding='utf-8'?>
<Project xmlns='http://schemas.microsoft.com/developer/msbuild/2003'>
    <PropertyGroup>
        <key1>value1</key1>
        <key2>value2</key2>
    </PropertyGroup>
</Project>"));
        }
    }
}