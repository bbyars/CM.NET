using System.IO;
using CM.Common;
using NUnit.Framework;

namespace CM.FunctionalTests.Common
{
    [TestFixture]
    public class PublishToSvnTest
    {
        [Test]
        public void ShouldImportTrunkAndBranchIfTrunkDoesNotExist()
        {
            Using.Directory("publishSvn", () =>
            Using.SvnRepo(url => 
            {
                File.WriteAllText("test.txt", "");

                var log = new TestLogger();
                var gateway = new SvnGateway(log);
                var publish = new PublishToSourceControl(gateway);
                publish.FromWorkingDirectory(".")
                    .WithMainline(url + "/trunk")
                    .WithCommitMessage("test")
                    .To(url + "/tags/v1");

                Assert.That(gateway.Exists(url + "/trunk/test.txt"), "not imported into trunk\n" + log.Contents);
                Assert.That(gateway.Exists(url + "/tags/v1/test.txt"), "not branched\n" + log.Contents);
            }));
        }
    }
}