using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace CM.FunctionalTests.MSBuild.Tasks
{
    [TestFixture]
    public class CopyToPhysicalDirectoryTest
    {
        [Test]
        public void ShouldCopyToVersionedTimestampedDirectory()
        {
            Using.Directory("copyToPhysicalDirectoryTest", () =>
            {
                Directory.CreateDirectory("source");
                File.WriteAllText(@"source\test.proj", @"
                    <Project DefaultTargets='test' xmlns='http://schemas.microsoft.com/developer/msbuild/2003'>
                        <UsingTask TaskName='CopyToPhysicalDirectory' AssemblyFile='$(MSBuildProjectDirectory)\..\..\CM.MSBuild.Tasks.dll' />

                        <Target Name='test'>
                            <CopyToPhysicalDirectory Server='localhost' SourceDirectory='.'
                                    DestinationDirectory='$(MSBuildProjectDirectory)\..\destination'>
                                <Output TaskParameter='PhysicalDirectory' PropertyName='PhysicalDirectory' />
                            </CopyToPhysicalDirectory>

                            <WriteLinesToFile File='$(MSBuildProjectDirectory)\physicalDirectory.txt' Lines='$(PhysicalDirectory)' />
                        </Target>
                    </Project>");
                var output = RunMSBuild(@"source\test.proj");

                Assert.That(File.Exists(@"source\physicalDirectory.txt"), "directory not written to file: " + output);
                var physicalDirectory = Path.GetFullPath(File.ReadAllText(@"source\physicalDirectory.txt"));
                Assert.That(Directory.Exists(physicalDirectory), physicalDirectory + " not created: " + output);
                var files = Directory.GetFiles(physicalDirectory).Select(path => Path.GetFileName(path)).ToArray();
                Assert.That(files, Is.EqualTo(new[] { "test.proj" }), "file not copied: " + output);
            });
        }

        // Should only keep N copies
        // Move RunMSBuild 
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