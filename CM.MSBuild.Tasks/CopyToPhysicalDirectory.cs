using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CM.Common;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace CM.MSBuild.Tasks
{
    /// <summary>
    /// When deploying a directory to an IIS virtual directory, it's often
    /// advantageous to create a new directory and point the virtual directory to it
    /// rather than overwrite an existing physical directory.  In addition to avoiding
    /// intermittent file locks, this approach allows easy rollback.  The CopyToPhysicalDirectory
    /// task enables this approach by always deploying to a new, timestamped directory.
    /// It also ensures that only a certain number of previously deployed physical directories are kept.
    /// </summary>
    /// <example>
    /// The following example will copy the SourceDirectory to C:\inetpub\wwwroot\Web-timestamp on somehost,
    /// where timestamp will be a number, such that newer deployments have a higher timestamp.
    /// The output property UncPhysicalDirectory will be \\somehost\C$\inetpub\wwwroot\Web-timestamp,
    /// and the output property LocalPhysicalDirectory will be C:\inetpub\wwwroot\Web-timestamp.
    /// Generally it is the LocalPhysicalDirectory that you want to pass to IIS when creating a virtual directory.
    /// 
    /// &lt;CopyToPhysicalDirectory SourceDirectory=&quot;$(PackageDirectory)\Web&quot;
    ///     Server=&quot;somehost&quot; DestinationDirectory=&quot;C:\inetpub\wwwroot\Web&quot;
    ///     NumberOfOldDeploysToKeep=&quot;2&quot;&gt;
    ///   &lt;Output TaskParameter=&quot;UncPhysicalDirectory&quot; PropertyName=&quot;UncPhysicalDirectory&quot; /&gt;
    ///   &lt;Output TaskParameter=&quot;LocalPhysicalDirectory&quot; PropertyName=&quot;LocalPhysicalDirectory&quot; /&gt;
    /// &lt;/CopyToPhysicalDirectory&gt;
    /// </example>
    public class CopyToPhysicalDirectory : Task
    {
        private readonly long timestamp;

        public CopyToPhysicalDirectory()
        {
            timestamp = DateTime.Now.Ticks;
        }

        /// <summary>
        /// The directory containing the web files you want to deploy.
        /// </summary>
        [Required]
        public virtual string SourceDirectory { get; set; }

        /// <summary>
        /// The web server name or IP address.
        /// </summary>
        [Required]
        public virtual string Server { get; set; }

        /// <summary>
        /// The (non-timestamped) name of the physical directory.  Keep the 
        /// directory name in a local format (e.g. C:\inetpub) rather than a 
        /// UNC format (e.g. \\host\C$\inetpub).  A timestamp will be appended
        /// to the name you give.
        /// </summary>
        [Required]
        public virtual string DestinationDirectory { get; set; }

        /// <summary>
        /// The number of previous deployments to keep.  Any additional timestamped
        /// physical directories above this number will be removed in chronological 
        /// order (the earliest timestamp will be removed first).
        /// </summary>
        public virtual int NumberOfOldDeploysToKeep { get; set; }

        /// <summary>
        /// The actual UNC path of the physical directory, including the timestamp.
        /// </summary>
        [Output]
        public virtual string UncPhysicalDirectory
        {
            get { return string.Format("{0}-{1}", new NetworkDirectory(Server, DestinationDirectory).NetworkPath, timestamp ); }
        }

        /// <summary>
        /// The actual local path of the physical directory, including the timestamp.
        /// Generally, this is the value you want to pass IIS when creating a virtual directory.
        /// </summary>
        [Output]
        public virtual string LocalPhysicalDirectory
        {
            get { return string.Format("{0}-{1}", DestinationDirectory, timestamp); }
        }

        public override bool Execute()
        {
            try
            {
                Log.LogMessage("Deploying to {0}", UncPhysicalDirectory);
                DeleteOldDeploys();
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
            var matchingDirectories = GetMatchingDirectories();
            SortByTimestamp(matchingDirectories);
            DeleteOldestDeploys(matchingDirectories);
        }

        private List<string> GetMatchingDirectories()
        {
            var directoryWithoutTimestamp = new NetworkDirectory(Server, DestinationDirectory);
            return Directory.GetDirectories(
                directoryWithoutTimestamp.ParentDirectoryName, directoryWithoutTimestamp.BaseDirectoryName + "-*")
                .Where(dir => Regex.IsMatch(dir, directoryWithoutTimestamp.BaseDirectoryName + @"-\d+$")).ToList();
        }

        private static void SortByTimestamp(List<string> matchingDirectories)
        {
            matchingDirectories.Sort((first, second) =>
            {
                var firstTimestamp = long.Parse(Regex.Match(first, @"\d+$").Value);
                var secondTimestamp = long.Parse(Regex.Match(second, @"\d+$").Value);
                return firstTimestamp.CompareTo(secondTimestamp);
            });
        }

        private void DeleteOldestDeploys(IList<string> matchingDirectories)
        {
            var numberOfDirectoriesToDelete = matchingDirectories.Count - NumberOfOldDeploysToKeep;
            for (var i = 0; i < numberOfDirectoriesToDelete; i++)
            {
                Log.LogMessage("Deleting previously deployed directory {0}", matchingDirectories[i]);
                Directory.Delete(matchingDirectories[i], true);
            }
        }
    }
}