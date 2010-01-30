using System;
using System.IO;
using CM.Common;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace CM.FunctionalTests.scripts
{
    [TestFixture]
    public class TeamCityTest : CMTest
    {
        [Test]
        public void ShouldFixInferredProperties()
        {
            Using.Directory("TeamCityTest", () =>
            {
                File.WriteAllText("Test.sln", "");
                Directory.CreateDirectory("Test.UnitTests");
                Directory.CreateDirectory("Test.FunctionalTests");

                File.WriteAllText("Test.proj.teamcity.patch", @"
                    <Project DefaultTargets='Clean' xmlns='http://schemas.microsoft.com/developer/msbuild/2003'>
                      <PropertyGroup>
                        <BUILD_NUMBER>1.2.3.4</BUILD_NUMBER>
                        <MSBuildCommunityTasksPath>..\CM.NET</MSBuildCommunityTasksPath>

                        <PostCleanTargets>TraceTeamCityData</PostCleanTargets>
                      </PropertyGroup>

                      <Import Project='..\CM.NET\MasterWorkflow.targets' />
                      <Import Project='..\CM.NET\TeamCity.targets' />
                    </Project>");
                var output = Shell.MSBuild("Test.proj.teamcity.patch", TimeSpan.FromSeconds(30));

                Assert.That(output, Text.Contains("IsInContinuousIntegration: true"));
                Assert.That(output, Text.Contains("Version: 1.2.3.4"));
                Assert.That(output, Text.Contains("Solution: Test.sln"));
                Assert.That(output, Text.Contains("UnitTestProject: Test.UnitTests"));
                Assert.That(output, Text.Contains("FunctionalTestProject: Test.FunctionalTests"));
            });
        }
    }
}