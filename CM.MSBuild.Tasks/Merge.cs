using System.IO;
using System.Linq;

namespace CM.MSBuild.Tasks
{
    public class Merge
    {
        private readonly DirectoryInfo sourceDirectory;
        private string[] excludedDirectories = new string[0];

        public static Merge From(string sourceDirectory)
        {
            return new Merge(new DirectoryInfo(sourceDirectory));
        }

        public Merge(DirectoryInfo sourceDirectory)
        {
            this.sourceDirectory = sourceDirectory;
        }

        public virtual Merge ExcludingDirectories(params string[] directories)
        {
            excludedDirectories = directories;
            return this;
        }
        
        public virtual Merge Into(string destinationDirectory)
        {
            AddOrUpdateFiles(destinationDirectory);
            DeleteRemovedFiles(destinationDirectory);
            AddNewSubdirectories(destinationDirectory);
            DeleteRemovedDirectories(destinationDirectory);
            MergeSubdirectories(destinationDirectory);
            return this;
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
            var directoriesToRemove = destinationDirectories.Where(
                dir => !sourceDirectories.Contains(dir) && !excludedDirectories.Contains(dir)).ToList();
            directoriesToRemove.ForEach(dir => Directory.Delete(Path.Combine(destinationDirectory, dir), true));
        }

        private void MergeSubdirectories(string destinationDirectory)
        {
            foreach (var subdirectory in sourceDirectory.GetDirectories())
            {
                var merge = new Merge(subdirectory).ExcludingDirectories(excludedDirectories);
                merge.Into(Path.Combine(destinationDirectory, subdirectory.Name));
            }
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