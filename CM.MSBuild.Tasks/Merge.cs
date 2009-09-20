using System;
using System.IO;
using System.Linq;

namespace CM.MSBuild.Tasks
{
    public class Merge
    {
        private readonly DirectoryInfo sourceDirectory;
        private string[] excludedDirectories = new string[0];
        private Action<string> addFileCallback = delegate { };
        private Action<string> changeFileCallback = delegate { };
        private Action<string> deleteFileCallback = delegate { };
        private string parentPath = "";

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
        
        public virtual Merge OnNewFiles(Action<string> callback)
        {
            addFileCallback = callback;
            return this;
        }

        public virtual Merge OnChangedFiles(Action<string> callback)
        {
            changeFileCallback = callback;
            return this;
        }

        public virtual Merge OnDeletedFiles(Action<string> callback)
        {
            deleteFileCallback = callback;
            return this;
        }

        public virtual Merge Into(string destinationDirectory)
        {
            UpdateChangedFiles(destinationDirectory);
            AddNewFiles(destinationDirectory);
            DeleteRemovedFiles(destinationDirectory);
            AddNewSubdirectories(destinationDirectory);
            DeleteRemovedDirectories(destinationDirectory);
            MergeSubdirectories(destinationDirectory);
            return this;
        }

        private void UpdateChangedFiles(string destinationDirectory)
        {
            foreach (var file in sourceDirectory.GetFiles())
            {
                var destinationFile = Path.Combine(destinationDirectory, file.Name);
                if (File.Exists(destinationFile))
                {
                    file.CopyTo(destinationFile, true);

                    // This is always called, even if the file hasn't changed.
                    // I haven't bothered fixing that because I can't think of any
                    // reason to pay the cost of file compares.  We won't do anything
                    // here in svn, and in git it won't matter if we call extra git adds.
                    changeFileCallback(Path.Combine(parentPath, file.Name));
                }
            }
        }

        private void AddNewFiles(string destinationDirectory)
        {
            var sourceFiles = GetFilenames(sourceDirectory.FullName);
            var destinationFiles = GetFilenames(destinationDirectory);
            var filesToAdd = sourceFiles.Where(filename => !destinationFiles.Contains(filename)).ToList();

            foreach (var file in filesToAdd)
            {
                File.Copy(Path.Combine(sourceDirectory.FullName, file), Path.Combine(destinationDirectory, file));
                addFileCallback(Path.Combine(parentPath, file));
            }
        }

        private void DeleteRemovedFiles(string destinationDirectory)
        {
            var sourceFiles = GetFilenames(sourceDirectory.FullName);
            var destinationFiles = GetFilenames(destinationDirectory);
            var filesToRemove = destinationFiles.Where(filename => !sourceFiles.Contains(filename)).ToList();

            foreach (var file in filesToRemove)
            {
                File.Delete(Path.Combine(destinationDirectory, file));
                deleteFileCallback(Path.Combine(parentPath, file));
            }
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
                var merge = new Merge(subdirectory)
                    .ExcludingDirectories(excludedDirectories)
                    .OnNewFiles(addFileCallback)
                    .OnChangedFiles(changeFileCallback)
                    .OnDeletedFiles(deleteFileCallback)
                    .WithParentPath(Path.Combine(parentPath, subdirectory.Name));
                merge.Into(Path.Combine(destinationDirectory, subdirectory.Name));
            }
        }

        private Merge WithParentPath(string path)
        {
            parentPath = path;
            return this;
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