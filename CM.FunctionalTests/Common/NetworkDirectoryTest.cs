using System.IO;
using CM.Common;
using NUnit.Framework;

namespace CM.FunctionalTests.Common
{
    [TestFixture]
    public class NetworkDirectoryTest
    {
        [Test]
        public void EnsureExistsAndIsEmptyShouldCreateDirIfNeeded()
        {
            Using.Directory("NetworkDirectoryTest", () =>
            {
                var directory = new NetworkDirectory("localhost", "subdir");
                directory.EnsureExistsAndIsEmpty();
                Assert.IsTrue(Directory.Exists("subdir"));
            });
        }

        [Test]
        public void EnsureExistsAndIsEmptyShouldCreateParentDirIfNeeded()
        {
            Using.Directory("NetworkDirectoryTest", () =>
            {
                var directory = new NetworkDirectory("localhost", @"subdir\subdir2");
                directory.EnsureExistsAndIsEmpty();
                Assert.IsTrue(Directory.Exists(@"subdir\subdir2"));
            });
        }

        [Test]
        public void EnsureExistsAndIsEmptyShouldRecursivelyEmptyIfNeeded()
        {
            Using.Directory("NetworkDirectoryTest", () =>
            {
                Directory.CreateDirectory("subdir");
                File.WriteAllText("file.txt", "test");
                File.WriteAllText(@"subdir\file.txt", "test");

                var directory = new NetworkDirectory("localhost", ".");
                directory.EnsureExistsAndIsEmpty();
                Assert.AreEqual(0, Directory.GetFiles(".").Length);
                Assert.AreEqual(0, Directory.GetDirectories(".").Length);
            });
        }

        [Test]
        public void MirrorShouldCopyFilesRecursively()
        {
            Using.Directory("NetworkDirectoryTest", () =>
            {
                Directory.CreateDirectory(@"Source\Subdir");
                File.WriteAllText(@"Source\RootFile.txt", "root file");
                File.WriteAllText(@"Source\Subdir\SubdirFile.txt", "subdir file");

                var directory = new NetworkDirectory("localhost", "Destination");
                directory.MirrorFrom("Source");
                Assert.IsTrue(File.Exists(@"Destination\RootFile.txt"));
                Assert.IsTrue(File.Exists(@"Destination\Subdir\SubdirFile.txt"));
            });
        }

        [Test]
        public void MirrorWithAbsolutePaths()
        {
            Using.Directory("NetworkDirectoryTest", () =>
            {
                Directory.CreateDirectory(@"Source\Subdir");
                File.WriteAllText(@"Source\RootFile.txt", "root file");
                File.WriteAllText(@"Source\Subdir\SubdirFile.txt", "subdir file");

                var directory = new NetworkDirectory("localhost", Path.GetFullPath("Destination"));
                directory.MirrorFrom(Path.GetFullPath("Source"));
                Assert.IsTrue(File.Exists(@"Destination\RootFile.txt"));
                Assert.IsTrue(File.Exists(@"Destination\Subdir\SubdirFile.txt"));
            });
        }
    }
}