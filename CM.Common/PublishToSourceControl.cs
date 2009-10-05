using System.IO;

namespace CM.Common
{
    public class PublishToSourceControl
    {
        private readonly ISourceControlProvider sourceControl;

        private string workingDirectory;
        private string mainlineUrl;
        private string mainlineWorkingDirectory;
        private string commitMessage;

        public PublishToSourceControl(ISourceControlProvider sourceControl)
        {
            this.sourceControl = sourceControl;
        }

        public virtual PublishToSourceControl WithCommitMessage(string commitMessage)
        {
            this.commitMessage = commitMessage;
            return this;
        }

        public virtual PublishToSourceControl FromWorkingDirectory(string workingDirectory)
        {
            this.workingDirectory = workingDirectory;
            return this;
        }

        public virtual PublishToSourceControl WithMainline(string mainlineUrl)
        {
            this.mainlineUrl = mainlineUrl;
            return this;
        }

        public virtual PublishToSourceControl To(string branchUrl)
        {
            mainlineWorkingDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            try
            {
                Publish(branchUrl);
            }
            finally
            {
                if (Directory.Exists(mainlineWorkingDirectory))
                    Shell.RmDir(mainlineWorkingDirectory);
            }

            return this;
        }

        private void Publish(string urlToPublishTo)
        {
            if (sourceControl.Exists(mainlineUrl))
            {
                sourceControl.CreateWorkingDirectory(mainlineUrl, mainlineWorkingDirectory);
                MergeToMainlineWorkingDirectory();
                sourceControl.Commit(mainlineWorkingDirectory, commitMessage);
            }
            else
            {
                sourceControl.Import(workingDirectory, mainlineUrl, commitMessage);
            }

            sourceControl.Branch(mainlineUrl, urlToPublishTo, commitMessage);
        }

        private void MergeToMainlineWorkingDirectory()
        {
            Merge.From(workingDirectory)
                .ExcludingDirectories(sourceControl.MetadataDirectories)
                .OnNewFiles(file => sourceControl.AddFile(file, mainlineWorkingDirectory))
                .OnNewDirectories(dir => sourceControl.AddDirectory(dir, mainlineWorkingDirectory))
                .OnChangedFiles(file => sourceControl.UpdateFile(file, mainlineWorkingDirectory))
                .OnDeletedFiles(file => sourceControl.DeleteFile(file, mainlineWorkingDirectory))
                .OnDeletedDirectories(dir => sourceControl.DeleteDirectory(dir, mainlineWorkingDirectory))
                .Into(mainlineWorkingDirectory);
        }
    }
}