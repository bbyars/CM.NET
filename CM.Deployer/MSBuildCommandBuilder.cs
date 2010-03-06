using System;
using System.Linq;
using CM.Common;

namespace CM.Deployer
{
    public class MSBuildCommandBuilder : IDeployCommandBuilder
    {
        public const string ConfigFile = "DeployConfig.properties";

        private readonly FileSystem fileSystem;

        public MSBuildCommandBuilder(FileSystem fileSystem, string msBuildExePath, string projectFilePath, string target)
        {
            this.fileSystem = fileSystem;
            CommandLine = string.Format("\"{0}\" \"{1}\" /t:{2} /p:\"PackageDirectory=.\" /p:\"ConfigPath={3}\"",
                msBuildExePath, projectFilePath, target, ConfigFile);
        }

        public string CommandLine { get; private set; }

        public virtual void SetEnvironmentProperties(PropertyList properties)
        {
            var propertyLines = properties.Select(p => string.Format("<{0}>{1}</{0}>", p.Key, p.Value)).ToArray();
            fileSystem.WriteAllText(ConfigFile, string.Format(@"<?xml version='1.0' encoding='utf-8'?>
<Project xmlns='http://schemas.microsoft.com/developer/msbuild/2003'>
    <PropertyGroup>
        {0}
    </PropertyGroup>
</Project>", string.Join(Environment.NewLine + "        ", propertyLines)));
        }
    }
}