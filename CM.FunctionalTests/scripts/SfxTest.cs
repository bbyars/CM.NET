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
        [SetUp]
        public void PrepareCMDirectory()
        {
            var cmFiles = new[] {"CM.Common.dll", "CM.MSBuild.Tasks.dll", "deployer.exe"};
            foreach (var file in cmFiles)
                File.Copy(file, Path.Combine("CM.NET", file), true);

            File.Copy(@"..\..\..\CM.Deploy.UI\App.config", @"CM.NET\deployer.exe.config", true);
        }

        [Test]
        public void ShouldSpliceEnvironmentsFileAndMSBuildFilenameIntoDeployerConfigFile()
        {
            Using.Directory("sfx-test", () =>
            {
                File.WriteAllText("test.proj", @"
                    <Project DefaultTargets='Build' xmlns='http://schemas.microsoft.com/developer/msbuild/2003'>
                      <PropertyGroup>
                        <CMDirectory>$(MSBuildProjectDirectory)\..\CM.NET</CMDirectory>
                        <EnvironmentsDirectory>$(MSBuildProjectDirectory)\env</EnvironmentsDirectory>
                      </PropertyGroup>

                      <Import Project='$(CMDirectory)\MasterWorkflow.targets' />
                      <Import Project='$(CMDirectory)\Sfx.targets' />
                    </Project>");

                var output = Shell.MSBuild("test.proj", TimeSpan.FromSeconds(5));
                var config = File.ReadAllText(@"build\package\deployer.exe.config");
                Assert.That(config, Text.Contains("<value>test.proj</value>"), output);
                Assert.That(config, Text.Contains("<value>env</value>"), output);
            });
        }

        [Test]
        public void ShouldCreateSelfExtractingExecutableThatRunsTheDeployer()
        {
            Using.Directory("sfx-test", () =>
            {
                File.WriteAllText("test.proj", @"
                    <Project DefaultTargets='Build' xmlns='http://schemas.microsoft.com/developer/msbuild/2003'>
                      <PropertyGroup>
                        <PackageName>SfxTest</PackageName>
                        <CMDirectory>$(MSBuildProjectDirectory)\..\CM.NET</CMDirectory>
                      </PropertyGroup>

                      <Import Project='$(CMDirectory)\MasterWorkflow.targets' />
                      <Import Project='$(CMDirectory)\Sfx.targets' />
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