using System;
using System.IO;
using CM.Common;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace CM.FunctionalTests.MSBuild.Tasks
{
    [TestFixture]
    public class ChangeDirectoryPrefixTest
    {
        [Test]
        public void ShouldChangePrefixOfAbsolutePaths()
        {
            Using.Directory("ChangeDirectoryPrefixTest", () =>
            {
                Directory.CreateDirectory("includes");
                File.WriteAllText(@"includes\first.txt", "");
                File.WriteAllText(@"includes\second.txt", "");

                File.WriteAllText("test.proj", @"
                    <Project DefaultTargets='test' xmlns='http://schemas.microsoft.com/developer/msbuild/2003'>
                        <UsingTask TaskName='ChangeDirectoryPrefix' AssemblyFile='$(MSBuildProjectDirectory)\..\CM.MSBuild.Tasks.dll' />

                        <Target Name='test'>
                            <ItemGroup>
                                <TestFiles Include='$(MSBuildProjectDirectory)\includes\*' />
                            </ItemGroup>
                            <ChangeDirectoryPrefix Files='@(TestFiles)' FromPrefix='$(MSBuildProjectDirectory)' ToPrefix='C:'>
                                <Output TaskParameter='TransformedFiles' ItemName='ChangedFiles' />
                            </ChangeDirectoryPrefix>

                            <Message Text='Changed to %(ChangedFiles.Identity)' />
                        </Target>
                    </Project>");
                var output = Shell.MSBuild(@"test.proj", TimeSpan.FromSeconds(10));

                Assert.That(output, Text.Contains(@"C:\includes\first.txt"));
                Assert.That(output, Text.Contains(@"C:\includes\second.txt"));
            });
        }

        [Test]
        public void ShouldChangePrefixOfRelativePaths()
        {
            Using.Directory("ChangeDirectoryPrefixTest", () =>
            {
                Directory.CreateDirectory("includes");
                File.WriteAllText(@"includes\first.txt", "");
                File.WriteAllText(@"includes\second.txt", "");

                File.WriteAllText("test.proj", @"
                    <Project DefaultTargets='test' xmlns='http://schemas.microsoft.com/developer/msbuild/2003'>
                        <UsingTask TaskName='ChangeDirectoryPrefix' AssemblyFile='$(MSBuildProjectDirectory)\..\CM.MSBuild.Tasks.dll' />

                        <Target Name='test'>
                            <ItemGroup>
                                <TestFiles Include='includes\*' />
                            </ItemGroup>
                            <ChangeDirectoryPrefix Files='@(TestFiles)' FromPrefix='$(MSBuildProjectDirectory)\includes' ToPrefix='C:'>
                                <Output TaskParameter='TransformedFiles' ItemName='ChangedFiles' />
                            </ChangeDirectoryPrefix>

                            <Message Text='Changed to %(ChangedFiles.Identity)' />
                        </Target>
                    </Project>");
                var output = Shell.MSBuild(@"test.proj", TimeSpan.FromSeconds(10));

                Assert.That(output, Text.Contains(@"C:\first.txt"));
                Assert.That(output, Text.Contains(@"C:\second.txt"));
            });
        }
    }
}