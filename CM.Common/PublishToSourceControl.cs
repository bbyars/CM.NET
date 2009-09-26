using System.IO;

namespace CM.Common
{
    public class PublishToSourceControl
    {
        private readonly ISourceControlGateway gateway;

        private string workingDirectory;
        private string mainlineUrl;
        private string mainlineWorkingDirectory;
        private string commitMessage;

        public PublishToSourceControl(ISourceControlGateway gateway)
        {
            this.gateway = gateway;
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
            if (gateway.Exists(mainlineUrl))
            {
                gateway.CreateWorkingDirectory(mainlineUrl, mainlineWorkingDirectory);
                MergeToMainlineWorkingDirectory();
                gateway.Commit(mainlineWorkingDirectory, commitMessage);
            }
            else
            {
                gateway.Import(workingDirectory, mainlineUrl, commitMessage);
            }

            gateway.Branch(mainlineUrl, urlToPublishTo, commitMessage);
        }

        private void MergeToMainlineWorkingDirectory()
        {
            Merge.From(workingDirectory)
                .ExcludingDirectories(gateway.MetadataDirectories)
                .OnNewFiles(file => gateway.AddFile(file, mainlineWorkingDirectory))
                .OnNewDirectories(dir => gateway.AddDirectory(dir, mainlineWorkingDirectory))
                .OnChangedFiles(file => gateway.UpdateFile(file, mainlineWorkingDirectory))
                .OnDeletedFiles(file => gateway.DeleteFile(file, mainlineWorkingDirectory))
                .OnDeletedDirectories(dir => gateway.DeleteDirectory(dir, mainlineWorkingDirectory))
                .Into(mainlineWorkingDirectory);
        }
    }
}