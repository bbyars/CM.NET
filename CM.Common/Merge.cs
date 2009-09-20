using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CM.Common
{
    public class Merge
    {
        private readonly DirectoryInfo sourceDirectory;
        private string destinationDirectory;
        private string[] sourceFiles;
        private string[] destinationFiles;
        private string[] sourceDirectories;
        private string[] destinationDirectories;
        private string[] excludedDirectories = new string[0];
        private string parentPath = "";

        private Action<string> addFileCallback = delegate { };
        private Action<string> changeFileCallback = delegate { };
        private Action<string> deleteFileCallback = delegate { };
        private Action<string> addDirectoryCallback = delegate { };
        private Action<string> deleteDirectoryCallback = delegate { };

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

        public virtual Merge OnNewDirectories(Action<string> callback)
        {
            addDirectoryCallback = callback;
            return this;
        }

        public virtual Merge OnDeletedDirectories(Action<string> callback)
        {
            deleteDirectoryCallback = callback;
            return this;
        }

        public virtual Merge Into(string destinationDirectory)
        {
            this.destinationDirectory = destinationDirectory;
            sourceFiles = GetFilenames(sourceDirectory.FullName);
            destinationFiles = GetFilenames(destinationDirectory);
            sourceDirectories = GetDirectories(sourceDirectory);
            destinationDirectories = GetDirectories(new DirectoryInfo(destinationDirectory));

            UpdateChangedFiles();
            AddNewFiles();
            DeleteRemovedFiles();
            AddNewSubdirectories();
            DeleteRemovedDirectories();
            MergeSubdirectories();

            return this;
        }

        private void UpdateChangedFiles()
        {
            var newFiles = sourceFiles.Except(destinationFiles);
            var removedFiles = destinationFiles.Except(sourceFiles);
            DoMerge(sourceFiles, newFiles.Union(removedFiles), changeFileCallback,
                file => File.Copy(Path.Combine(sourceDirectory.FullName, file), Path.Combine(destinationDirectory, file), true));
        }

        private void AddNewFiles()
        {
            DoMerge(sourceFiles, destinationFiles, addFileCallback,
                file => File.Copy(Path.Combine(sourceDirectory.FullName, file), Path.Combine(destinationDirectory, file)));
        }

        private void DeleteRemovedFiles()
        {
            DoMerge(destinationFiles, sourceFiles, deleteFileCallback,
                file => File.Delete(Path.Combine(destinationDirectory, file)));
        }

        private void AddNewSubdirectories()
        {
            DoMerge(sourceDirectories, destinationDirectories, addDirectoryCallback,
                dir => Directory.CreateDirectory(Path.Combine(destinationDirectory, dir)));
        }

        private void DeleteRemovedDirectories()
        {
            DoMerge(destinationDirectories, sourceDirectories.Union(excludedDirectories), deleteDirectoryCallback,
                dir => Directory.Delete(Path.Combine(destinationDirectory, dir), true));
        }

        private void DoMerge(IEnumerable<string> baseObjects, IEnumerable<string> excludeObjects,
            Action<string> callback, Action<string> mergeOperation)
        {
            var fileSystemObjects = baseObjects.Except(excludeObjects).ToList();

            foreach (var fileSystemObject in fileSystemObjects)
            {
                mergeOperation(fileSystemObject);
                callback(Path.Combine(parentPath, fileSystemObject));
            }
        }

        private void MergeSubdirectories()
        {
            foreach (var subdirectory in sourceDirectory.GetDirectories())
            {
                var merge = new Merge(subdirectory)
                    .ExcludingDirectories(excludedDirectories)
                    .OnNewFiles(addFileCallback)
                    .OnChangedFiles(changeFileCallback)
                    .OnDeletedFiles(deleteFileCallback)
                    .OnNewDirectories(addDirectoryCallback)
                    .OnDeletedDirectories(deleteDirectoryCallback)
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