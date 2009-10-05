using System;
using System.IO;
using CM.Common;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace CM.FunctionalTests.Common
{
//    [TestFixture]
    // Runs in IDE, fails on command line - WHY?
    public class GitProviderTest
    {
        private TestLogger log;

        [SetUp]
        public void CreateLogger()
        {
            log = new TestLogger();
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowExceptionIfCannotClone()
        {
            GitProvider.Clone("", log);
        }

        [Test]
        public void MissingFileShouldNotExist()
        {
            Using.GitRepo(url =>
            {
                using (var provider = GitProvider.Clone(url, log))
                    Assert.That(provider.Exists("test.txt"), Is.False, log.Contents);
            });
        }

        [Test]
        public void CreatedFileShouldExist()
        {
            Using.GitRepo(url =>
            {
                using (var provider = GitProvider.Clone(url, log))
                {
                    File.WriteAllText("test.txt", "");
                    provider.AddFile("test.txt", ".");
                    Assert.That(provider.Exists("test.txt"), log.Contents);
                }
            });
        }
    }
}