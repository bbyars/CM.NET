using System;
using System.IO;
using System.Linq;
using CM.Common;
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
                                <Output TaskParameter='UncPhysicalDirectory' PropertyName='UncPhysicalDirectory' />
                            </CopyToPhysicalDirectory>

                            <WriteLinesToFile File='$(MSBuildProjectDirectory)\physicalDirectory.txt' Lines='$(UncPhysicalDirectory)' />
                        </Target>
                    </Project>");
                var output = Shell.MSBuild(@"source\test.proj", TimeSpan.FromSeconds(10));

                Assert.That(File.Exists(@"source\physicalDirectory.txt"), "directory not written to file: " + output);
                var physicalDirectory = Path.GetFullPath(File.ReadAllText(@"source\physicalDirectory.txt"));
                Assert.That(Directory.Exists(physicalDirectory), physicalDirectory + " not created: " + output);
                var files = Directory.GetFiles(physicalDirectory).Select(path => Path.GetFileName(path)).ToArray();
                Assert.That(files, Is.EqualTo(new[] { "test.proj" }), "file not copied: " + output);
            });
        }

        [Test]
        public void ShouldOnlyKeepNCopiesOfPhysicalDirectory()
        {
            Using.Directory("copyToPhysicalDirectoryTest", () =>
            {
                Directory.CreateDirectory("source");
                File.WriteAllText(@"source\test.proj", @"
                    <Project DefaultTargets='test' xmlns='http://schemas.microsoft.com/developer/msbuild/2003'>
                        <UsingTask TaskName='CopyToPhysicalDirectory' AssemblyFile='$(MSBuildProjectDirectory)\..\..\CM.MSBuild.Tasks.dll' />

                        <Target Name='test'>
                            <CopyToPhysicalDirectory Server='localhost' SourceDirectory='.' NumberOfOldDeploysToKeep='1'
                                DestinationDirectory='$(MSBuildProjectDirectory)\..\destination' />
                        </Target>
                    </Project>");
                var output = RunProjectThreeTimes(@"source\test.proj");

                var directories = Directory.GetDirectories(".", "destination-*");
                Assert.That(directories.Length, Is.EqualTo(2), "did not delete directories:\n" + output);
            });
        }

        [Test]
        public void ShouldCompareOnlyTimestampWhenDeletingPreviousDeploys()
        {
            Using.Directory("copyToPhysicalDirectoryTest", () =>
            {
                Directory.CreateDirectory("source");
                Directory.CreateDirectory("destination-fooledYou");
                File.WriteAllText(@"source\test.proj", @"
                    <Project DefaultTargets='test' xmlns='http://schemas.microsoft.com/developer/msbuild/2003'>
                        <UsingTask TaskName='CopyToPhysicalDirectory' AssemblyFile='$(MSBuildProjectDirectory)\..\..\CM.MSBuild.Tasks.dll' />

                        <Target Name='test'>
                            <CopyToPhysicalDirectory Server='localhost' SourceDirectory='.' NumberOfOldDeploysToKeep='0'
                                DestinationDirectory='$(MSBuildProjectDirectory)\..\destination' />
                        </Target>
                    </Project>");
                var output = Shell.MSBuild(@"source\test.proj", TimeSpan.FromSeconds(10));

                Assert.That(Directory.Exists("destination-fooledYou"), "deleted wrong directory:\n" + output);
            });
        }

        private static string RunProjectThreeTimes(string project)
        {
            var output = "";
            for (var i = 0; i < 3; i++)
                output += Shell.MSBuild(project, TimeSpan.FromSeconds(10)) + "\n\n";
            return output;
        }
    }
}