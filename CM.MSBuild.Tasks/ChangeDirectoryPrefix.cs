using System;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace CM.MSBuild.Tasks
{
    /// <summary>
    /// Transforms a list of items, that could be specified either with absolute or relative
    /// paths, into a new list of items with a new directory prefix.  I could find no way to 
    /// do this with standard MSBuild transforms.
    /// </summary>
    /// <example>
    /// The following example will transform either $(MSBuildProjectDirectory)\test.proj or
    /// test.proj to $(PackageDirectory)\test.proj, putting the transformed value in 
    /// the ItemGroup named ChangedFiles:
    /// 
    /// &lt;ItemGroup&gt;
    ///   &lt;Projects Include=&quot;test.proj&quot; /&gt;
    /// &lt;/ItemGroup&gt;
    /// &lt;ChangeDirectoryPrefix Files=&quot;@(Projects)&quot; FromPrefix=&quot;$(MSBuildProjectDirectory)&quot; ToPrefix=&quot;$(PackageDirectory)&quot;&gt;
    ///   &lt;Output TaskParameter=&quot;TransformedFiles&quot; ItemName=&quot;ChangedFiles&quot; /&gt;
    /// &lt;/ChangeDirectoryPrefix&gt;
    /// </example>
    public class ChangeDirectoryPrefix : Task
    {
        /// <summary>
        /// The input list of files.  They can be included either with relative or absolute paths.
        /// </summary>
        [Required]
        public virtual ITaskItem[] Files { get; set; }

        /// <summary>
        /// The absolute directory prefix to remove.
        /// </summary>
        [Required]
        public virtual string FromPrefix { get; set; }

        /// <summary>
        /// The absolute directory prefix to change to.
        /// </summary>
        [Required]
        public virtual string ToPrefix { get; set; }

        /// <summary>
        /// The result of the transformation.
        /// </summary>
        [Output]
        public virtual ITaskItem[] TransformedFiles { get; set; }

        public override bool Execute()
        {
            try
            {
                TransformedFiles = Files.Select(item => 
                    new TaskItem(item.GetMetadata("FullPath").Replace(FromPrefix, ToPrefix))).ToArray();
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