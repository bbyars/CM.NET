using System.IO;
using CM.MSBuild.Tasks;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace CM.FunctionalTests.MSBuild.Tasks
{
    [TestFixture]
    public class SvnGatewayTest
    {
        private TestLogger log;

        [SetUp]
        public void CreateLogger()
        {
            log = new TestLogger();
        }

        [Test]
        public void UncreatedUrlShouldNotExist()
        {
            Assert.That(!new SvnGateway(log).Exists(@"file:///missing/svn/repo"), log.Contents);
        }

        [Test]
        public void CreatedUrlShouldExist()
        {
            Using.SvnRepo(url => Assert.That(new SvnGateway(log).Exists(url), log.Contents));
        }

        [Test]
        public void ShouldNotExistIfEndpointDoesNotExist()
        {
            Using.SvnRepo(url => Assert.That(!new SvnGateway(log).Exists(url + "/test"), log.Contents));
        }

        [Test]
        public void ImportShouldAddFilesToRepository()
        {
            Using.Directory("svn", () =>
            Using.SvnRepo(url =>
            {
                Directory.CreateDirectory("trunk");
                var gateway = new SvnGateway(log);
                gateway.Import(".", url + "/test", "");
                Assert.That(gateway.Exists(url + "/test/trunk"), log.Contents);
            }));
        }

        [Test]
        public void CreateWorkingDirectoryShouldPerformACheckout()
        {
            Using.Directory("svn", () =>
            Using.SvnRepo(url =>
            {
                var gateway = new SvnGateway(log);
                Directory.CreateDirectory("trunk");
                gateway.Import(".", url + "/test", "");

                gateway.CreateWorkingDirectory(url + "/test", "workingDir");
                Assert.That(Directory.Exists(@"workingDir\trunk"), log.Contents);
            }));
        }

        [Test]
        public void CommittingNewDirectoryShouldAddItToTheRepository()
        {
            Using.Directory("svn", () =>
            Using.SvnRepo(url =>
            {
                var gateway = new SvnGateway(log);
                gateway.Import(".", url + "/test", "");

                gateway.CreateWorkingDirectory(url + "/test", "workingDir");
                Directory.CreateDirectory(@"workingDir\trunk");
                gateway.AddDirectory("trunk", "workingDir");
                gateway.Commit("workingDir", "");

                Assert.That(gateway.Exists(url + "/test/trunk"), log.Contents);
            }));
        }

        [Test]
        public void CommittingDeletedDirectoryShouldRemoveItFromTheRepository()
        {
            Using.Directory("svn", () =>
            Using.SvnRepo(url =>
            {
                Directory.CreateDirectory("test");
                var gateway = new SvnGateway(log);
                gateway.Import(".", url + "/test", "");

                gateway.CreateWorkingDirectory(url + "/test", "workingDir");
                gateway.DeleteDirectory("test", "workingDir");
                gateway.Commit("workingDir", "");

                Assert.That(!gateway.Exists(url + "/test/test"), log.Contents);
            }));
        }

        [Test]
        public void CommittingNewFileShouldAddItToTheRepository()
        {
            Using.Directory("svn", () =>
            Using.SvnRepo(url =>
            {
                var gateway = new SvnGateway(log);
                gateway.Import(".", url + "/test", "");

                gateway.CreateWorkingDirectory(url + "/test", "workingDir");
                File.WriteAllText(@"workingDir\test.txt", "");
                gateway.AddFile("test.txt", "workingDir");
                gateway.Commit("workingDir", "");

                Assert.That(gateway.Exists(url + "/test/test.txt"), log.Contents);
            }));
        }

        [Test]
        public void CommittingDeletedFileShouldRemoveItFromTheRepository()
        {
            Using.Directory("svn", () =>
            Using.SvnRepo(url =>
            {
                File.WriteAllText("test.txt", "");
                var gateway = new SvnGateway(log);
                gateway.Import(".", url + "/test", "");

                gateway.CreateWorkingDirectory(url + "/test", "workingDir");
                gateway.DeleteFile("test.txt", "workingDir");
                gateway.Commit("workingDir", "");

                Assert.That(!gateway.Exists(url + "/test/test.txt"), log.Contents);
            }));
        }

        [Test]
        public void CommittingUpdatedFileShouldChangeItInTheRepository()
        {
            Using.Directory("svn", () =>
            Using.SvnRepo(url =>
            {
                File.WriteAllText("test.txt", "");
                var gateway = new SvnGateway(log);
                gateway.Import(".", url + "/test", "");

                gateway.CreateWorkingDirectory(url + "/test", "workingDir");
                File.WriteAllText(@"workingDir\test.txt", "update");
                gateway.Commit("workingDir", "");
                gateway.CreateWorkingDirectory(url + "/test", "validationDir");

                Assert.That(File.ReadAllText(@"validationDir\test.txt"), Is.EqualTo("update"), log.Contents);
            }));
        }

        [Test]
        public void BranchingShouldCopyRepositoryToNewEndpoint()
        {
            Using.Directory("svn", () =>
            Using.SvnRepo(url =>
            {
                File.WriteAllText("test.txt", "");
                var gateway = new SvnGateway(log);
                gateway.Import(".", url + "/test", "");

                gateway.Branch(url + "/test", url + "/branch", "");
                Assert.That(gateway.Exists(url + "/branch/test.txt"), log.Contents);
            }));
        }
    }
}