using System.IO;
using CM.MSBuild.Tasks;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace CM.FunctionalTests.MSBuild.Tasks
{
    [TestFixture]
    public class MergeTest
    {
        [SetUp]
        public void CreateDirectories()
        {
            Directory.CreateDirectory("old");
            Directory.CreateDirectory("new");
        }

        [TearDown]
        public void RemoveDirectories()
        {
            Directory.Delete("old", true);
            Directory.Delete("new", true);
        }
        
        [Test]
        public void MergingEmptyDirectoriesDoesNothing()
        {
            Merge.From("new").Into("old");
            Assert.That(Directory.GetFiles("old").Length, Is.EqualTo(0));
        }

        [Test]
        public void ShouldAddMissingTopLevelFile()
        {
            File.WriteAllText(@"new\test.txt", "");
            Merge.From("new").Into("old");

            Assert.That(Directory.GetFiles("old").Length, Is.EqualTo(1));
            Assert.That(File.Exists(@"old\test.txt"));
        }

        [Test]
        public void ShouldUpdateExistingTopLevelFile()
        {
            File.WriteAllText(@"old\test.txt", "old");
            File.WriteAllText(@"new\test.txt", "new");
            Merge.From("new").Into("old");

            Assert.That(File.ReadAllText(@"old\test.txt"), Is.EqualTo("new"));
        }

        [Test]
        public void ShouldDeleteRemovedTopLevelFile()
        {
            File.WriteAllText(@"old\test.txt", "");
            Merge.From("new").Into("old");

            Assert.That(Directory.GetFiles("old").Length, Is.EqualTo(0));
        }
    }
}