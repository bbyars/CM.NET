using System;
using CM.Common;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace CM.MSBuild.Tasks
{
    public class CopyToPhysicalDirectory : Task
    {
        private long timestamp;

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

        [Output]
        public virtual string PhysicalDirectory
        {
            get
            {
                return string.Format("{0}-{1}", new NetworkDirectory(Server, DestinationDirectory).NetworkPath, timestamp );
            }
        }

        public override bool Execute()
        {
            try
            {
                new NetworkDirectory(Server, PhysicalDirectory).MirrorFrom(SourceDirectory);
                return true;
            }
            catch (Exception e)
            {
                Log.LogErrorFromException(e);
                return false;
            }
        }
    }
}