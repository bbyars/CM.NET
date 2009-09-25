using System;
using System.IO;
using CM.Common;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace CM.FunctionalTests.Common
{
//    [TestFixture]
    // Runs in IDE, fails on command line - WHY?
    public class GitGatewayTest
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
            GitGateway.Clone("", log);
        }

        [Test]
        public void MissingFileShouldNotExist()
        {
            Using.GitRepo(url =>
            {
                using (var gateway = GitGateway.Clone(url, log))
                    Assert.That(gateway.Exists("test.txt"), Is.False, log.Contents);
            });
        }

        [Test]
        public void CreatedFileShouldExist()
        {
            Using.GitRepo(url =>
            {
                using (var gateway = GitGateway.Clone(url, log))
                {
                    File.WriteAllText("test.txt", "");
                    gateway.AddFile("test.txt", ".");
                    Assert.That(gateway.Exists("test.txt"), log.Contents);
                }
            });
        }
    }
}