using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using CM.Common;
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
                        <PackageName>SfxTest</PackageName>
                        <SevenZipDirectory>$(MSBuildProjectDirectory)\..\scripts\Dependencies\7-zip</SevenZipDirectory>
                        <CMDirectory>$(MSBuildProjectDirectory)\..</CMDirectory>
                      </PropertyGroup>

                      <ItemGroup>
                        <PackageFiles Include='$(MSBuildProjectFullPath)' />
                      </ItemGroup>

                      <Import Project='$(MSBuildProjectDirectory)\..\scripts\MasterWorkflow.targets' />
                      <Import Project='$(MSBuildProjectDirectory)\..\scripts\Sfx.targets' />
                    </Project>");

                var output = Shell.MSBuild("test.proj", TimeSpan.FromSeconds(5));
                Assert.That(File.Exists(@"build\sfx\SfxTest.exe"), output);

                var sfxProcess = new ProcessRunner(@"build\sfx\SfxTest.exe");
                sfxProcess.Start("");
                try
                {
                    Assert.That(WaitForProcess("deployer"), Is.Not.Null, "no deployer process is running");
                }
                finally
                {
                    sfxProcess.KillTree();
                }
            });
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
    }
}