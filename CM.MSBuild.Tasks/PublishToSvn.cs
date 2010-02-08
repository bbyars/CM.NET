using System;
using System.IO;
using CM.Common;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace CM.MSBuild.Tasks
{
    /// <summary>
    /// Publishes a set of files to a subversion repository.
    /// Generally, this would be the build artifacts for a library that 
    /// is to be consumed by other applications.  This task will merge
    /// the new artifacts to the trunk and copy the trunk to a new tag.
    /// Either the trunk or the tag can be consumed in an svn:externals
    /// </summary>
    /// <example>
    /// &lt;PublishToSvn TrunkUrl=&quot;svn://libraries/fizzbang/trunk&quot;
    ///   PublishedUrl=&quot;svn://libraries/fizzbang/tags/$(Version)&quot;
    ///   FilesToPublish=&quot;@(PackageFiles)&quot;
    ///   CommitMessage=&quot;auto publishing version $(Version)&quot; /&gt;
    /// </example>
    /// <remarks>
    /// If the TrunkUrl does not exist, it will automatically be imported.
    /// Typically, you would only publish on a successful build in continuous
    /// integration.  Before this will work, however, you will need to open a
    /// command prompt on the build agent machine, logged in as the build
    /// agent user, and do an svn command line operation, caching the 
    /// authentication.
    /// </remarks>
    public class PublishToSvn : Task
    {
        public PublishToSvn()
        {
            CommandTimeoutInSeconds = 600;
        }

        /// <summary>
        /// The URL to merge the new files to.
        /// </summary>
        [Required]
        public virtual string TrunkUrl { get; set; }

        /// <summary>
        /// The URL to publish the new files to, generally a tag.
        /// </summary>
        [Required]
        public virtual string PublishedUrl { get; set; }

        /// <summary>
        /// The files to publish
        /// </summary>
        [Required]
        public virtual ITaskItem[] FilesToPublish { get; set; }

        /// <summary>
        /// The message to be used when comitting changes.
        /// </summary>
        [Required]
        public virtual string CommitMessage { get; set; }

        /// <summary>
        /// The maximum amount of time, in seconds, to wait before an
        /// svn command finishes.  If the time is exceeded, the svn
        /// process will be killed and the publishing aborted.
        /// Defaults to 600 (10 minutes)
        /// </summary>
        public virtual int CommandTimeoutInSeconds { get; set; }

        public override bool Execute()
        {
            var workingDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            try
            {
                return Publish(workingDirectory);
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
        }

        private bool Publish(string workingDirectory)
        {
            CreateNewWorkingDirectory(workingDirectory);
            var logAdapter = new MSBuildLogAdapter(Log);
            var publish = new PublishToSourceControl(new SvnProvider(logAdapter, TimeSpan.FromSeconds(CommandTimeoutInSeconds)));
            publish.FromWorkingDirectory(workingDirectory)
                .WithMainline(TrunkUrl)
                .WithCommitMessage(CommitMessage)
                .To(PublishedUrl);
            return !logAdapter.HasErrors;
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