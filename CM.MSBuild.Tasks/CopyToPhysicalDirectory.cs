using System;
using Microsoft.Build.Framework;

namespace CM.MSBuild.Tasks
{
    public class CopyToPhysicalDirectory : CMTask
    {
        public virtual string SourceDirectory { get; set; }
        public virtual string Server { get; set; }
        public virtual string DestinationDirectory { get; set; }
        public virtual string Version { get; set; }

        [Output]
        public virtual string PhysicalDirectory
        {
            get
            {
                return string.Format("{0}-{1}-{2}",
                    new NetworkDirectory(Server, DestinationDirectory).NetworkPath, DateTime.Now.Ticks, Version);
            }
        }

        protected override void DoExecute()
        {
            new NetworkDirectory(Server, PhysicalDirectory).MirrorFrom(SourceDirectory);
        }
    }
}