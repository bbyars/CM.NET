using System.IO;
using CM.Common;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace CM.FunctionalTests.Common
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
            Assert.That(Directory.GetFiles("old"), Is.EqualTo(new string[0]));
        }

        [Test]
        public void ShouldAddMissingTopLevelFile()
        {
            File.WriteAllText(@"new\test.txt", "");
            Merge.From("new").Into("old");

            Assert.That(Directory.GetFiles("old"), Is.EqualTo(new[] {@"old\test.txt"}));
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

            Assert.That(Directory.GetFiles("old"), Is.EqualTo(new string[0]));
        }

        [Test]
        public void ShouldAddNewSubdirectory()
        {
            Directory.CreateDirectory(@"new\subdir");
            Merge.From("new").Into("old");

            Assert.That(Directory.GetDirectories("old"), Is.EqualTo(new[] {@"old\subdir"}));
        }

        [Test]
        public void ShouldDeleteRemovedSubdirectory()
        {
            Directory.CreateDirectory(@"old\subdir");
            Merge.From("new").Into("old");

            Assert.That(Directory.GetDirectories("old"), Is.EqualTo(new string[0]));
        }

        [Test]
        public void ShouldAddMissingFileInExistingSubdirectory()
        {
            Directory.CreateDirectory(@"old\subdir");
            Directory.CreateDirectory(@"new\subdir");
            File.WriteAllText(@"new\subdir\test.txt", "");
            Merge.From("new").Into("old");

            Assert.That(Directory.GetFiles(@"old\subdir"), Is.EqualTo(new[] {@"old\subdir\test.txt"}));
        }

        [Test]
        public void ShouldAddMissingSubdirectoryWithContents()
        {
            Directory.CreateDirectory(@"new\subdir");
            File.WriteAllText(@"new\subdir\test.txt", "");
            Merge.From("new").Into("old");

            Assert.That(Directory.GetFiles(@"old\subdir"), Is.EqualTo(new[] { @"old\subdir\test.txt" }));
        }

        [Test]
        public void ShouldUpdateExistingFileInSubdirectory()
        {
            Directory.CreateDirectory(@"old\subdir");
            Directory.CreateDirectory(@"new\subdir");
            File.WriteAllText(@"old\subdir\test.txt", "old");
            File.WriteAllText(@"new\subdir\test.txt", "new");
            Merge.From("new").Into("old");

            Assert.That(File.ReadAllText(@"old\subdir\test.txt"), Is.EqualTo("new"));
        }

        [Test]
        public void ShouldDeleteRemovedFileInSubdirectory()
        {
            Directory.CreateDirectory(@"old\subdir");
            Directory.CreateDirectory(@"new\subdir");
            File.WriteAllText(@"old\subdir\test.txt", "");
            Merge.From("new").Into("old");

            Assert.That(Directory.GetFiles(@"old\subdir"), Is.EqualTo(new string[0]));
        }

        [Test]
        public void ShouldRecursivelyDeleteRemovedDirectory()
        {
            Directory.CreateDirectory(@"old\subdir");
            File.WriteAllText(@"old\subdir\test.txt", "");
            Merge.From("new").Into("old");

            Assert.That(Directory.GetDirectories("old"), Is.EqualTo(new string[0]));
        }

        [Test]
        public void ShouldBeAbleToExcludeDirectories()
        {
            Directory.CreateDirectory(@"old\.svn");
            Merge.From("new").ExcludingDirectories(".svn").Into("old");

            Assert.That(Directory.GetDirectories("old"), Is.EqualTo(new[] {@"old\.svn"}));
        }

        [Test]
        public void ShouldKeepExcludeDirectoriesWhenRecursing()
        {
            Directory.CreateDirectory(@"old\subdir");
            Directory.CreateDirectory(@"old\subdir\.svn");
            Directory.CreateDirectory(@"new\subdir");
            Merge.From("new").ExcludingDirectories(".svn").Into("old");

            Assert.That(Directory.GetDirectories(@"old\subdir"), Is.EqualTo(new[] { @"old\subdir\.svn" }));
        }

        [Test]
        public void ShouldExecuteCallbackWhenAddingFiles()
        {
            File.WriteAllText(@"new\test.txt", "");
            var log = "";
            Merge.From("new").OnNewFiles(file => log = file).Into("old");

            Assert.That(log, Is.EqualTo("test.txt"));
        }

        [Test]
        public void ShouldExecuteCallbackAfterCopyingNewFile()
        {
            File.WriteAllText(@"new\test.txt", "");
            Merge.From("new").OnNewFiles(file => Assert.That(File.Exists(@"old\test.txt"))).Into("old");
        }

        [Test]
        public void ShouldIncludeParentDirectoryWhenCallingAddFileCallbackInSubdirectory()
        {
            Directory.CreateDirectory(@"new\subdir");
            File.WriteAllText(@"new\subdir\test.txt", "");
            var log = "";
            Merge.From("new").OnNewFiles(file => log = file).Into("old");

            Assert.That(log, Is.EqualTo(@"subdir\test.txt"));
        }

        [Test]
        public void ShouldNotExecuteAddFileCallbackWhenUpdatingFiles()
        {
            File.WriteAllText(@"old\test.txt", "old");
            File.WriteAllText(@"new\test.txt", "new");
            var log = "";
            Merge.From("new").OnNewFiles(file => log = file).Into("old");

            Assert.That(log, Is.EqualTo(""));
        }

        [Test]
        public void ShouldExecuteCallbackWhenUpdatingFiles()
        {
            File.WriteAllText(@"old\test.txt", "old");
            File.WriteAllText(@"new\test.txt", "new");
            var log = "";
            Merge.From("new").OnChangedFiles(file => log = file).Into("old");

            Assert.That(log, Is.EqualTo("test.txt"));
        }

        [Test]
        public void ShouldIncludeParentDirectoryWhenCallingUpdateFileCallbackInSubdirectory()
        {
            Directory.CreateDirectory(@"old\subdir");
            Directory.CreateDirectory(@"new\subdir");
            File.WriteAllText(@"old\subdir\test.txt", "old");
            File.WriteAllText(@"new\subdir\test.txt", "new");
            var log = "";
            Merge.From("new").OnChangedFiles(file => log = file).Into("old");

            Assert.That(log, Is.EqualTo(@"subdir\test.txt"));
        }

        [Test]
        public void ShouldExecuteCallbackWhenDeletingFiles()
        {
            File.WriteAllText(@"old\test.txt", "old");
            var log = "";
            Merge.From("new").OnDeletedFiles(file => log = file).Into("old");

            Assert.That(log, Is.EqualTo("test.txt"));
        }

        [Test]
        public void ShouldIncludeParentDirectoryWhenCallingDeleteFileCallbackInSubdirectory()
        {
            Directory.CreateDirectory(@"old\subdir");
            Directory.CreateDirectory(@"new\subdir");
            File.WriteAllText(@"old\subdir\test.txt", "old");
            var log = "";
            Merge.From("new").OnDeletedFiles(file => log = file).Into("old");

            Assert.That(log, Is.EqualTo(@"subdir\test.txt"));
        }

        [Test]
        public void ShouldExecuteCallbackWhenAddingDirectories()
        {
            Directory.CreateDirectory(@"new\subdir");
            var log = "";
            Merge.From("new").OnNewDirectories(dir => log = dir).Into("old");

            Assert.That(log, Is.EqualTo("subdir"));
        }

        [Test]
        public void ShouldIncludeParentDirectoryWhenCallingAddingDirectoryCallbackInSubdirectory()
        {
            Directory.CreateDirectory(@"old\subdir");
            Directory.CreateDirectory(@"new\subdir");
            Directory.CreateDirectory(@"new\subdir\test");
            var log = "";
            Merge.From("new").OnNewDirectories(dir => log = dir).Into("old");

            Assert.That(log, Is.EqualTo(@"subdir\test"));
        }

        [Test]
        public void ShouldExecuteCallbackWhenDeletingDirectories()
        {
            Directory.CreateDirectory(@"old\subdir");
            var log = "";
            Merge.From("new").OnDeletedDirectories(dir => log = dir).Into("old");

            Assert.That(log, Is.EqualTo("subdir"));
        }

        [Test]
        public void ShouldIncludeParentDirectoryWhenCallingDeleteDirectoryCallbackInSubdirectory()
        {
            Directory.CreateDirectory(@"old\subdir");
            Directory.CreateDirectory(@"old\subdir\test");
            Directory.CreateDirectory(@"new\subdir");
            var log = "";
            Merge.From("new").OnDeletedDirectories(dir => log = dir).Into("old");

            Assert.That(log, Is.EqualTo(@"subdir\test"));
        }
    }
}