using System;
using System.IO;
using CM.Common;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace CM.MSBuild.Tasks
{
    public class PublishToSvn : Task
    {
        [Required]
        public virtual string TrunkUrl { get; set; }

        [Required]
        public virtual string PublishedUrl { get; set; }

        [Required]
        public virtual ITaskItem[] FilesToPublish { get; set; }

        [Required]
        public virtual string CommitMessage { get; set; }

        public override bool Execute()
        {
            var workingDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            try
            {
                CreateNewWorkingDirectory(workingDirectory);
                var logAdapter = new MSBuildLogAdapter(Log);
                var publish = new PublishToSourceControl(new SvnGateway(logAdapter));
                publish.FromWorkingDirectory(workingDirectory)
                    .WithMainline(TrunkUrl)
                    .WithCommitMessage(CommitMessage)
                    .To(PublishedUrl);

                if (logAdapter.HasErrors)
                    return false;
            }
            catch (Exception e)
            {
                Log.LogErrorFromException(e);
                return false;
            }
            finally
            {
                if (Directory.Exists(workingDirectory))
                    Directory.Delete(workingDirectory, true);
            }

            return true;
        }

        private void CreateNewWorkingDirectory(string path)
        {
            foreach (var item in FilesToPublish)
            {
                var destinationDirectory = Path.Combine(path, item.GetMetadata("RecursiveDir"));
                if (!Directory.Exists(destinationDirectory))
                    Directory.CreateDirectory(destinationDirectory);

                var destinationFilename = item.GetMetadata("Filename") + item.GetMetadata("Extension");
                var destinationPath = Path.Combine(destinationDirectory, destinationFilename);
                File.Copy(item.GetMetadata("FullPath"), destinationPath);
            }
        }
    }
}