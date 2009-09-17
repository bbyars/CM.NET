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
                Assert.That(fileSystem.ListAllFilesIn(".", "*.config"), Is.EqualTo(new[] {"dev.config", "prod.config", "qa.config"}));
            });
        }

        [Test]
        public void ShouldFilterFilesBasedOnSearchPattern()
        {
            Using.Directory("fileSystemTest", () =>
            {
                File.WriteAllText("dev.config", "");
                File.WriteAllText("prod.properties", "");
                File.WriteAllText("qa.config", "");

                var fileSystem = new FileSystem();
                Assert.That(fileSystem.ListAllFilesIn(".", "*.config"), Is.EqualTo(new[] { "dev.config", "qa.config" }));
            });
        }

        [Test]
        public void ShouldReadAllText()
        {
            Using.Directory("fileSystemTest", () =>
            {
                File.WriteAllText("test.txt", "This is a test");
                var fileSystem = new FileSystem();
                Assert.That(fileSystem.ReadAllText("test.txt"), Is.EqualTo("This is a test"));
            });
        }
    }
}