using System.IO;
using System.Linq;

namespace CM.MSBuild.Tasks
{
    public class Merge
    {
        private readonly DirectoryInfo sourceDirectory;

        public static Merge From(string sourceDirectory)
        {
            return new Merge(new DirectoryInfo(sourceDirectory));
        }

        public Merge(DirectoryInfo sourceDirectory)
        {
            this.sourceDirectory = sourceDirectory;
        }

        public virtual void Into(string destinationDirectory)
        {
            AddOrUpdateFiles(destinationDirectory);
            DeleteRemovedFiles(destinationDirectory);
            AddNewSubdirectories(destinationDirectory);
            DeleteRemovedDirectories(destinationDirectory);
            MergeSubdirectories(destinationDirectory);
        }

        private void AddOrUpdateFiles(string destinationDirectory)
        {
            foreach (var file in sourceDirectory.GetFiles())
                file.CopyTo(Path.Combine(destinationDirectory, file.Name), true);
        }

        private void DeleteRemovedFiles(string destinationDirectory)
        {
            var sourceFiles = GetFilenames(sourceDirectory.FullName);
            var destinationFiles = GetFilenames(destinationDirectory);
            var filesToRemove = destinationFiles.Where(filename => !sourceFiles.Contains(filename)).ToList();
            filesToRemove.ForEach(filename => File.Delete(Path.Combine(destinationDirectory, filename)));
        }

        private void AddNewSubdirectories(string destinationDirectory)
        {
            var sourceDirectories = GetDirectories(sourceDirectory);
            var destinationDirectories = GetDirectories(new DirectoryInfo(destinationDirectory));
            var directoriesToAdd = sourceDirectories.Where(dir => !destinationDirectories.Contains(dir)).ToList();
            directoriesToAdd.ForEach(dir => Directory.CreateDirectory(Path.Combine(destinationDirectory, dir)));
        }

        private void DeleteRemovedDirectories(string destinationDirectory)
        {
            var sourceDirectories = GetDirectories(sourceDirectory);
            var destinationDirectories = GetDirectories(new DirectoryInfo(destinationDirectory));
            var directoriesToRemove = destinationDirectories.Where(dir => !sourceDirectories.Contains(dir)).ToList();
            directoriesToRemove.ForEach(dir => Directory.Delete(Path.Combine(destinationDirectory, dir)));
        }

        private void MergeSubdirectories(string destinationDirectory)
        {
            foreach (var subdirectory in sourceDirectory.GetDirectories())    
                new Merge(subdirectory).Into(Path.Combine(destinationDirectory, subdirectory.Name));
        }

        private static string[] GetFilenames(string directory)
        {
            return Directory.GetFiles(directory).Select(path => Path.GetFileName(path)).ToArray();
        }

        private static string[] GetDirectories(DirectoryInfo directory)
        {
            return directory.GetDirectories().Select(dir => dir.Name).ToArray();
        }
    }
}