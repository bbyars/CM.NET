using System;
using System.Collections.Generic;
using System.IO;
using CM.Common;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace CM.MSBuild.Tasks
{
    public class CopyToPhysicalDirectory : Task
    {
        private readonly long timestamp;

        public CopyToPhysicalDirectory()
        {
            timestamp = DateTime.Now.Ticks;
        }

        [Required]
        public virtual string SourceDirectory { get; set; }

        [Required]
        public virtual string Server { get; set; }

        [Required]
        public virtual string DestinationDirectory { get; set; }

        public virtual int NumberOfOldDeploysToKeep { get; set; }

        [Output]
        public virtual string UncPhysicalDirectory
        {
            get { return string.Format("{0}-{1}", new NetworkDirectory(Server, DestinationDirectory).NetworkPath, timestamp ); }
        }

        [Output]
        public virtual string LocalPhysicalDirectory
        {
            get { return string.Format("{0}-{1}", DestinationDirectory, timestamp); }
        }

        public override bool Execute()
        {
            try
            {
                DeleteOldDeploys();
                Log.LogMessage("Deploying to {0}", UncPhysicalDirectory);
                new NetworkDirectory(Server, UncPhysicalDirectory).MirrorFrom(SourceDirectory);
                return true;
            }
            catch (Exception e)
            {
                Log.LogErrorFromException(e);
                return false;
            }
        }

        private void DeleteOldDeploys()
        {
            var directoryWithoutTimestamp = new NetworkDirectory(Server, DestinationDirectory);
            var matchingDirectories = new List<string>(Directory.GetDirectories(
                directoryWithoutTimestamp.ParentDirectoryName, directoryWithoutTimestamp.BaseDirectoryName + "-*"));
            if (matchingDirectories.Count > NumberOfOldDeploysToKeep)
            {
                SortByTimestamp(matchingDirectories);
                var numberOfDirectoriesToDelete = matchingDirectories.Count - NumberOfOldDeploysToKeep;
                for (var i = 0; i < numberOfDirectoriesToDelete; i++)
                {
                    Log.LogMessage("Deleting previously deployed directory {0}", matchingDirectories[i]);
                    Directory.Delete(matchingDirectories[i], true);
                }
            }
        }

        private void SortByTimestamp(List<string> matchingDirectories)
        {
            var directoryWithoutTimestamp = new NetworkDirectory(Server, DestinationDirectory).NetworkPath;
            matchingDirectories.Sort((first, second) =>
            {
                var firstTimestamp = long.Parse(first.Replace(directoryWithoutTimestamp + "-", ""));
                var secondTimestamp = long.Parse(second.Replace(directoryWithoutTimestamp + "-", ""));
                return firstTimestamp.CompareTo(secondTimestamp);
            });
        }
    }
}