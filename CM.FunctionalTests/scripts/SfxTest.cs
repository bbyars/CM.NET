using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace CM.FunctionalTests.scripts
{
    [TestFixture]
    public class SfxTest
    {
        [Test]
        public void ShouldCreateSelfExtractingExecutableThatRunsTheDeployer()
        {
            Using.Directory("sfx-test", () =>
            {
                File.WriteAllText("test.proj", @"
                    <Project DefaultTargets='Build' xmlns='http://schemas.microsoft.com/developer/msbuild/2003'>
                      <PropertyGroup>
                        <ProjectName>SfxTest</ProjectName>
                        <Version>1.2.3.4</Version>
                        <SevenZipDirectory>..\scripts\Dependencies\7-zip</SevenZipDirectory>
                      </PropertyGroup>

                      <ItemGroup>
                        <DeployExe Include='..\deployer.exe' />
                      </ItemGroup>

                      <Import Project='..\scripts\Master.targets' />
                      <Import Project='..\scripts\Sfx.targets' />
                    </Project>");

                var output = RunMSBuild("test.proj");
                Assert.That(File.Exists(@"sfx\SfxTest-1.2.3.4.exe"), output);

                var sfxProcess = Process.Start(@"sfx\SfxTest-1.2.3.4.exe");
                try
                {
                    var deployerProcess = WaitForProcess("deployer");
                    Assert.That(deployerProcess, Is.Not.Null, "no deployer process is running");
                    deployerProcess.Kill();
                }
                finally
                {
                    sfxProcess.Kill();
                    WaitForProcessExit("SfxTest-1.2.3.4");
                }
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

        private static Process WaitForProcess(string processName)
        {
            var maxWait = DateTime.Now.AddSeconds(3);
            var processes = Process.GetProcessesByName(processName);
            while (processes.Length == 0 && DateTime.Now < maxWait)
            {
                Thread.Sleep(200);
                processes = Process.GetProcessesByName(processName);
            }
            return processes.Length == 0 ? null : processes[0];
        }

        private static void WaitForProcessExit(string processName)
        {
            var maxWait = DateTime.Now.AddSeconds(3);
            var processes = Process.GetProcessesByName(processName);
            while (processes.Length > 1 && DateTime.Now < maxWait)
            {
                Thread.Sleep(200);
                processes = Process.GetProcessesByName(processName);
            }
        }
    }
}