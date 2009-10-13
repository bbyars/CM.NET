using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using CM.Common;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace CM.FunctionalTests.scripts
{
    [TestFixture]
    public class SfxTest : CMTest
    {
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
        public void ShouldCopyCMFiles_EnvironmentFiles_DeployerFiles_AndProjectFile_ToPackage()
        {
            Using.Directory("sfx-test", () =>
            {
                // sfx.targets quite reasonably expects all CM.NET files to be underneath the project fie
                XCopyCMFiles();

                Directory.CreateDirectory("env");
                File.WriteAllText(@"env\dev.properties", "");
                File.WriteAllText(@"env\test.properties", "");

                File.WriteAllText("test.proj", @"
                    <Project DefaultTargets='Build' xmlns='http://schemas.microsoft.com/developer/msbuild/2003'>
                      <PropertyGroup>
                        <CMDirectory>$(MSBuildProjectDirectory)\CM.NET</CMDirectory>
                        <EnvironmentsDirectory>$(MSBuildProjectDirectory)\env</EnvironmentsDirectory>
                      </PropertyGroup>

                      <Import Project='$(CMDirectory)\MasterWorkflow.targets' />
                      <Import Project='$(CMDirectory)\Sfx.targets' />
                    </Project>");
                var output = Shell.MSBuild("test.proj", TimeSpan.FromSeconds(30));

                var expectedFiles = new List<string>(Directory.GetFiles("CM.NET"));
                expectedFiles.AddRange(new[] {@"test.proj", @"env\dev.properties", @"env\test.properties",
                    "CM.Common.dll", "deployer.exe", "deployer.exe.config"});
                expectedFiles.Sort();

                var actualFiles = GetAllPackageFiles();
                actualFiles.Sort();

                Assert.That(actualFiles, Is.EqualTo(expectedFiles),
                    string.Format("Package files not what was expected\nExpected: {0}\nActual {1}\nMSBuild: {2}",
                        string.Join(", ", expectedFiles.ToArray()), string.Join(", ", actualFiles.ToArray()), output));
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

                var processRunner = new ProcessRunner();
                var process = processRunner.Start(@"build\sfx\SfxTest.exe");
                try
                {
                    Assert.That(WaitForProcess("deployer"), Is.Not.Null, "no deployer process is running");
                }
                finally
                {
                    process.KillTree();
                }
            });
        }

        private static void XCopyCMFiles()
        {
            var startInfo = new ProcessStartInfo { FileName = "xcopy", Arguments = @"..\CM.NET CM.NET\", WindowStyle = ProcessWindowStyle.Hidden };
            Process.Start(startInfo).WaitForExit();
        }

        private static List<string> GetAllPackageFiles()
        {
            var actualFiles = new List<string>(Directory.GetFiles(@"build\package"));
            foreach (var subdirectory in Directory.GetDirectories(@"build\package"))
                actualFiles.AddRange(Directory.GetFiles(subdirectory));
            actualFiles = actualFiles.Select(file => file.Replace(@"build\package\", "")).ToList();
            return actualFiles;
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