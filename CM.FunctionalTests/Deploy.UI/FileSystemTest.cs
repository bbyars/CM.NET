using System.IO;
using CM.Deploy.UI;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace CM.FunctionalTests.Deploy.UI
{
    [TestFixture]
    public class FileSystemTest
    {
        [Test]
        public void ShouldListAllFilesInDirectory()
        {
            Using.Directory("fileSystemTest", () =>
            {
                File.WriteAllText("dev.config", "");
                File.WriteAllText("prod.config", "");
                File.WriteAllText("qa.config", "");

                var fileSystem = new FileSystem();
                Assert.That(fileSystem.ListAllFilesIn("."), Is.EqualTo(new[] {"dev.config", "prod.config", "qa.config"}));
            });
        }
    }
}