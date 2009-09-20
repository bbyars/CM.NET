using System;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace CM.MSBuild.Tasks
{
    public class PublishToSvn : Task
    {
        private readonly SvnGateway gateway;

        public PublishToSvn() : this(new SvnGateway())
        {
        }

        public PublishToSvn(SvnGateway gateway)
        {
            this.gateway = gateway;
        }

        [Required]
        public virtual string TrunkUrl { get; set; }

        [Required]
        public virtual string PublishedUrl { get; set; }

        [Required]
        public virtual ITaskItem[] FilesToPublish { get; set; } 

        public override bool Execute()
        {
            var newWorkingDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var trunkWorkingDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            try
            {
                Publish(newWorkingDirectory, trunkWorkingDirectory);
            }
            catch (Exception e)
            {
                Log.LogErrorFromException(e);
                return false;
            }
            finally
            {
                if (Directory.Exists(newWorkingDirectory))
                    Directory.Delete(newWorkingDirectory, true);
                if (Directory.Exists(trunkWorkingDirectory))
                    Directory.Delete(trunkWorkingDirectory);
            }

            return true;
        }

        private void Publish(string newWorkingDirectory, string trunkWorkingDirectory)
        {
            CreateNewWorkingDirectory(newWorkingDirectory);
            if (gateway.Exists(TrunkUrl))
            {
                gateway.CreateWorkingDirectory(TrunkUrl, trunkWorkingDirectory);
                MergeToTrunkWorkingDirectory(newWorkingDirectory, trunkWorkingDirectory);
                gateway.Commit(trunkWorkingDirectory, "");
            }
            else
            {
                gateway.Import(newWorkingDirectory, TrunkUrl, "");
            }
            gateway.Branch(TrunkUrl, PublishedUrl, "");
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

        private void MergeToTrunkWorkingDirectory(string newWorkingDirectory, string trunkWorkingDirectory)
        {
            Merge.From(newWorkingDirectory)
                .ExcludingDirectories(".svn")
                .OnNewFiles(file => gateway.AddFile(file, trunkWorkingDirectory))
                .OnNewDirectories(dir => gateway.AddDirectory(dir, trunkWorkingDirectory))
                .OnChangedFiles(file => gateway.UpdateFile(file, trunkWorkingDirectory))
                .OnDeletedFiles(file => gateway.DeleteFile(file, trunkWorkingDirectory))
                .OnDeletedDirectories(dir => gateway.DeleteDirectory(dir, trunkWorkingDirectory))
                .Into(trunkWorkingDirectory);
        }
    }
}