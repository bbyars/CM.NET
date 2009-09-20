using System.IO;
using CM.MSBuild.Tasks;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace CM.FunctionalTests.MSBuild.Tasks
{
    [TestFixture]
    public class SvnGatewayTest
    {
        [Test]
        public void UncreatedUrlShouldNotExist()
        {
            Assert.That(new SvnGateway().Exists(@"file:///missing/svn/repo"), Is.False);
        }

        [Test]
        public void CreatedUrlShouldExist()
        {
            Using.SvnRepo(url => Assert.That(new SvnGateway().Exists(url)));
        }

        [Test]
        public void ShouldNotExistIfEndpointDoesNotExist()
        {
            Using.SvnRepo(url => Assert.That(new SvnGateway().Exists(url + "/test"), Is.False));
        }

        [Test]
        public void ImportShouldAddFilesToRepository()
        {
            Using.Directory("svn", () =>
            Using.SvnRepo(url =>
            {
                Directory.CreateDirectory("trunk");
                var gateway = new SvnGateway();
                gateway.Import(".", url + "/test", "");
                Assert.That(gateway.Exists(url + "/test/trunk"));
            }));
        }

        [Test]
        public void CreateWorkingDirectoryShouldPerformACheckout()
        {
            Using.Directory("svn", () =>
            Using.SvnRepo(url =>
            {
                var gateway = new SvnGateway();
                Directory.CreateDirectory("trunk");
                gateway.Import(".", url + "/test", "");

                gateway.CreateWorkingDirectory(url + "/test", "workingDir");
                Assert.That(Directory.Exists(@"workingDir\trunk"));
            }));
        }

        [Test]
        public void CommittingNewDirectoryShouldAddItToTheRepository()
        {
            Using.Directory("svn", () =>
            Using.SvnRepo(url =>
            {
                var gateway = new SvnGateway();
                gateway.Import(".", url + "/test", "");

                gateway.CreateWorkingDirectory(url + "/test", "workingDir");
                Directory.CreateDirectory(@"workingDir\trunk");
                gateway.AddDirectory("trunk", "workingDir");
                gateway.Commit("workingDir", "");

                Assert.That(gateway.Exists(url + "/test/trunk"));
            }));
        }
    }
}