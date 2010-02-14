using System;
using System.IO;
using CM.Common;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace CM.FunctionalTests.scripts
{
    [TestFixture]
    public class ZipTest : CMTest
    {
        [Test]
        public void ShouldZipAllFilesInPackageDirectory()
        {
            Using.Directory("zip-test", () =>
            {
                File.WriteAllText("first.txt", "");
                Directory.CreateDirectory("subdir");
                File.WriteAllText(@"subdir\second.txt", "");

                File.WriteAllText("test.proj", @"
                    <Project DefaultTargets='Build' xmlns='http://schemas.microsoft.com/developer/msbuild/2003'>
                      <PropertyGroup>
                        <Version>1.2.3.4</Version>
                        <MSBuildCommunityTasksPath>$(MSBuildProjectDirectory)\..\CM.NET</MSBuildCommunityTasksPath>

                        <PackageTargets>CopyPackageFiles</PackageTargets>
                      </PropertyGroup>

                      <Import Project='..\CM.NET\Default.targets' />
                      <Import Project='..\CM.NET\Packagers\Zip.targets' />

                      <Target Name='CopyPackageFiles'>
                        <ItemGroup>
                          <PackageFiles Include='**\*.txt' />
                        </ItemGroup>

                        <Copy SourceFiles='@(PackageFiles)'
                          DestinationFiles=""@(PackageFiles->'$(PackageDirectory)\%(RecursiveDir)%(Filename)%(Extension)')"" />
                      </Target>
                    </Project>");
                var output = Shell.MSBuild("test.proj", TimeSpan.FromSeconds(30));

                Assert.That(File.Exists(@"build\test-1.2.3.4.zip"), "zip not created: " + output);

                var contents = ArchiveContents(@"build\test-1.2.3.4.zip");
                Assert.That(contents, Text.Contains("first.txt"));
                Assert.That(contents, Text.Contains(@"subdir\second.txt"));
            });
        }

        private static string ArchiveContents(string archivePath)
        {
            var process = new ProcessRunner().Exec(@"..\CM.NET\7z l " + archivePath, TimeSpan.FromSeconds(10));
            return process.StandardOutput;
        }
    }
}