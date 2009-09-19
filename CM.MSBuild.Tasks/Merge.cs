using System.IO;
using System.Linq;

namespace CM.MSBuild.Tasks
{
    public class Merge
    {
        private readonly DirectoryInfo sourceDirectory;

        public static Merge From(string sourceDirectory)
        {
            return new Merge(sourceDirectory);
        }

        public Merge(string sourceDirectory)
        {
            this.sourceDirectory = new DirectoryInfo(sourceDirectory);
        }

        public virtual void Into(string destinationDirectory)
        {
            AddOrUpdateFiles(destinationDirectory);
            DeleteRemovedFiles(destinationDirectory);
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

        private static string[] GetFilenames(string directory)
        {
            return Directory.GetFiles(directory).Select(path => Path.GetFileName(path)).ToArray();
        }
    }
}