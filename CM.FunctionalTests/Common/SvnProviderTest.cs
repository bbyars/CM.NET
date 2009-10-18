using System;
using System.IO;
using CM.Common;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace CM.FunctionalTests.Common
{
    [TestFixture]
    public class SvnProviderTest
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
            Assert.That(!new SvnProvider(log, TimeSpan.FromMinutes(10)).Exists(@"file:///missing/svn/repo"), log.Contents);
        }

        [Test]
        public void CreatedUrlShouldExist()
        {
            Using.SvnRepo(url => Assert.That(new SvnProvider(log, TimeSpan.FromMinutes(10)).Exists(url), log.Contents));
        }

        [Test]
        public void ShouldNotExistIfEndpointDoesNotExist()
        {
            Using.SvnRepo(url => Assert.That(!new SvnProvider(log, TimeSpan.FromMinutes(10)).Exists(url + "/test"), log.Contents));
        }

        [Test]
        public void ImportShouldAddFilesToRepository()
        {
            Using.SvnRepo(url =>
            {
                Directory.CreateDirectory("trunk");
                var provider = new SvnProvider(log, TimeSpan.FromMinutes(10));
                provider.Import(".", url + "/test", "");
                Assert.That(provider.Exists(url + "/test/trunk"), log.Contents);
            });
        }

        [Test]
        public void CreateWorkingDirectoryShouldPerformACheckout()
        {
            Using.SvnRepo(url =>
            {
                var provider = new SvnProvider(log, TimeSpan.FromMinutes(10));
                Directory.CreateDirectory("trunk");
                provider.Import(".", url + "/test", "");

                provider.CreateWorkingDirectory(url + "/test", "workingDir");
                Assert.That(Directory.Exists(@"workingDir\trunk"), log.Contents);
            });
        }

        [Test]
        public void CommittingNewDirectoryShouldAddItToTheRepository()
        {
            Using.SvnRepo(url =>
            {
                var provider = new SvnProvider(log, TimeSpan.FromMinutes(10));
                provider.Import(".", url + "/test", "");

                provider.CreateWorkingDirectory(url + "/test", "workingDir");
                Directory.CreateDirectory(@"workingDir\trunk");
                provider.AddDirectory("trunk", "workingDir");
                provider.Commit("workingDir", "");

                Assert.That(provider.Exists(url + "/test/trunk"), log.Contents);
            });
        }

        [Test]
        public void CommittingDeletedDirectoryShouldRemoveItFromTheRepository()
        {
            Using.SvnRepo(url =>
            {
                Directory.CreateDirectory("test");
                var provider = new SvnProvider(log, TimeSpan.FromMinutes(10));
                provider.Import(".", url + "/test", "");

                provider.CreateWorkingDirectory(url + "/test", "workingDir");
                provider.DeleteDirectory("test", "workingDir");
                provider.Commit("workingDir", "");

                Assert.That(!provider.Exists(url + "/test/test"), log.Contents);
            });
        }

        [Test]
        public void CommittingNewFileShouldAddItToTheRepository()
        {
            Using.SvnRepo(url =>
            {
                var provider = new SvnProvider(log, TimeSpan.FromMinutes(10));
                provider.Import(".", url + "/test", "");

                provider.CreateWorkingDirectory(url + "/test", "workingDir");
                File.WriteAllText(@"workingDir\test.txt", "");
                provider.AddFile("test.txt", "workingDir");
                provider.Commit("workingDir", "");

                Assert.That(provider.Exists(url + "/test/test.txt"), log.Contents);
            });
        }

        [Test]
        public void CommittingDeletedFileShouldRemoveItFromTheRepository()
        {
            Using.SvnRepo(url =>
            {
                File.WriteAllText("test.txt", "");
                var provider = new SvnProvider(log, TimeSpan.FromMinutes(10));
                provider.Import(".", url + "/test", "");

                provider.CreateWorkingDirectory(url + "/test", "workingDir");
                provider.DeleteFile("test.txt", "workingDir");
                provider.Commit("workingDir", "");

                Assert.That(!provider.Exists(url + "/test/test.txt"), log.Contents);
            });
        }

        [Test]
        public void CommittingUpdatedFileShouldChangeItInTheRepository()
        {
            Using.SvnRepo(url =>
            {
                File.WriteAllText("test.txt", "");
                var provider = new SvnProvider(log, TimeSpan.FromMinutes(10));
                provider.Import(".", url + "/test", "");

                provider.CreateWorkingDirectory(url + "/test", "workingDir");
                File.WriteAllText(@"workingDir\test.txt", "update");
                provider.Commit("workingDir", "");
                provider.CreateWorkingDirectory(url + "/test", "validationDir");

                Assert.That(File.ReadAllText(@"validationDir\test.txt"), Is.EqualTo("update"), log.Contents);
            });
        }

        [Test]
        public void BranchingShouldCopyRepositoryToNewEndpoint()
        {
            Using.SvnRepo(url =>
            {
                File.WriteAllText("test.txt", "");
                var provider = new SvnProvider(log, TimeSpan.FromMinutes(10));
                provider.Import(".", url + "/test", "");

                provider.Branch(url + "/test", url + "/branch", "");

                Assert.That(provider.Exists(url + "/branch/test.txt"), log.Contents);
            });
        }

        [Test]
        public void BranchingShouldCreateParentDirectories()
        {
            Using.SvnRepo(url =>
            {
                File.WriteAllText("test.txt", "");
                var provider = new SvnProvider(log, TimeSpan.FromMinutes(10));
                provider.Import(".", url + "/test", "");

                provider.Branch(url + "/test", url + "/branch/v1", "");
                Assert.That(provider.Exists(url + "/branch/v1/test.txt"), log.Contents);
            });
        }

        [Test]
        public void ImportingShouldCreateParentDirectories()
        {
            Using.SvnRepo(url =>
            {
                File.WriteAllText("test.txt", "");
                var provider = new SvnProvider(log, TimeSpan.FromMinutes(10));
                provider.Import(".", url + "/test/trunk", "");

                Assert.That(provider.Exists(url + "/test/trunk/test.txt"), log.Contents);
            });
        }
    }
}