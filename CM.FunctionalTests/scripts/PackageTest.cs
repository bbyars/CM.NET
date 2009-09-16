using System.Diagnostics;
using System.IO;
using NUnit.Framework;

namespace CM.FunctionalTests.scripts
{
    [TestFixture]
    public class PackageTest
    {
        [Test]
        public void ShouldArchiveAllFilesInPackageDirectory()
        {
            Using.Directory("package-test", () =>
            {
                File.WriteAllText("test.proj", @"
                    <Project DefaultTargets='Build' xmlns='http://schemas.microsoft.com/developer/msbuild/2003'>
                      <PropertyGroup>
                        <ProjectName>PackageTest</ProjectName>
                        <Version>1.2.3.4</Version>
                        <MSBuildCommunityTasksPath>$(MSBuildProjectDirectory)\..\scripts\Dependencies\msbuild-community-tasks</MSBuildCommunityTasksPath>
                      </PropertyGroup>

                      <PropertyGroup>
                        <PackageTargets>$(PackageTargets);CopyFiles</PackageTargets>
                      </PropertyGroup>

                      <Import Project='$(MSBuildCommunityTasksPath)\MSBuild.Community.Tasks.Targets' />
                      <Import Project='..\scripts\Master.targets' />
                      <Import Project='..\scripts\Package.targets' />

                      <Target Name='CopyFiles'>
                        <Copy SourceFiles='test.proj' DestinationFolder='$(PackageDirectory)' />
                      </Target>
                    </Project>");

                var output = RunMsBuild("test.proj");
                Assert.That(File.Exists(@"package\PackageTest-1.2.3.4.zip"), output);
            });
        }

        private static string RunMsBuild(string msbuildFilename)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = @"C:\Windows\Microsoft.NET\Framework\v3.5\MSBuild.exe",
                Arguments = msbuildFilename,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            var process = Process.Start(startInfo);
            process.WaitForExit(5000);
            return process.StandardOutput.ReadToEnd();
        }
    }
}