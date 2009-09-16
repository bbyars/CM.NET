using System.Diagnostics;
using System.IO;
using NUnit.Framework;

namespace CM.FunctionalTests.scripts
{
    [TestFixture]
    public class PackageTest
    {
        [Test]
        public void ShouldCreateSelfExtractingExecutable()
        {
            Using.Directory("package-test", () =>
            {
                File.WriteAllText("test.proj", @"
                    <Project DefaultTargets='Build' xmlns='http://schemas.microsoft.com/developer/msbuild/2003'>
                      <PropertyGroup>
                        <ProjectName>PackageTest</ProjectName>
                        <Version>1.2.3.4</Version>
                        <DeployExeDirectory>..</DeployExeDirectory>
                        <SevenZipDirectory>..\scripts\Dependencies\7-zip</SevenZipDirectory>
                      </PropertyGroup>

                      <Import Project='..\scripts\Master.targets' />
                      <Import Project='..\scripts\Package.targets' />
                    </Project>");

                var output = RunMSBuild("test.proj");
                Assert.That(File.Exists(@"sfx\PackageTest-1.2.3.4.exe"), output);
            });
        }

        private static string RunMSBuild(string msbuildFilename)
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