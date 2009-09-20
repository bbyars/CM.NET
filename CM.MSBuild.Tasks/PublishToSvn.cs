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
            try
            {
                CreateWorkingDirectory(@"C:\dev\CM.NET\tmp");
            }
            catch (Exception e)
            {
                Log.LogErrorFromException(e);
                return false;
            }
//            if (TrunkExists())
//            {
//                CheckoutTrunk();
//                MergeToTrunkWorkingDirectory();
//                CommitTrunk();
//            }
//            else
//            {
//                ImportTrunk();
//            }
//
//            CopyTrunkToPublishedUrl();
            return true;
        }

        public void CreateWorkingDirectory(string path)
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

        private bool TrunkExists()
        {
            throw new NotImplementedException();
        }

        private void CheckoutTrunk()
        {
            throw new NotImplementedException();
        }

        private void MergeToTrunkWorkingDirectory()
        {
            throw new NotImplementedException();
        }

        private void CommitTrunk()
        {
            throw new NotImplementedException();
        }

        private void ImportTrunk()
        {
            throw new NotImplementedException();
        }

        private void CopyTrunkToPublishedUrl()
        {
            throw new NotImplementedException();
        }
    }
}