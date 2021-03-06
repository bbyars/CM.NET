using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using CM.Common;

namespace CM.Deployer
{
    /// <summary>
    /// This environment-loading strategy expects you to place a series of MSBuild files
    /// inside an environments directory.  Each MSBuild file should contain a PropertyGroup
    /// containing all environment-specific properties (with the same property names in
    /// each file).  Anything not in a PropertyGroup will be ignored (it would be a security
    /// loophole otherwise, since we don't show it in the GUI).  You will have an opportunity
    /// to change any properties at deploy time.
    /// </summary>
    public class EnvironmentFilesLoader : IEnvironmentLoader
    {
        private const string MSBuildNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";

        private readonly FileSystem fileSystem;
        private readonly string environmentsDirectory;
        private readonly string configurationFileExtension;

        public EnvironmentFilesLoader(FileSystem fileSystem, string environmentsDirectory, string configurationFileExtension)
        {
            this.fileSystem = fileSystem;
            this.environmentsDirectory = environmentsDirectory;
            this.configurationFileExtension = configurationFileExtension;
        }

        public virtual string[] GetEnvironments()
        {
            var files = fileSystem.ListAllFilesIn(environmentsDirectory, "*." + configurationFileExtension);
            return files.Select(file => Path.GetFileNameWithoutExtension(file)).ToArray();
        }

        public virtual PropertyList GetProperties(string environment)
        {
            if (string.IsNullOrEmpty(environment))
                return new PropertyList();

            var filename = string.Format("{0}.{1}", environment, configurationFileExtension);
            var path = Path.Combine(environmentsDirectory, filename);
            return LoadProperties(path);
        }

        public virtual PropertyList LoadProperties(string path)
        {
            var xml = XElement.Parse(fileSystem.ReadAllText(path));
            var result = new PropertyList();
            var propertyNodes = xml.Descendants(ScopedName("PropertyGroup")).Descendants();
            foreach (var node in propertyNodes)
                result.Add(node.Name.LocalName, node.Value);
            return result;
        }

        public virtual void SaveProperties(PropertyList properties, string path)
        {
            var propertyLines = properties.Select(p => string.Format("<{0}>{1}</{0}>", p.Key, p.Value)).ToArray();
            fileSystem.WriteAllText(path, string.Format(@"<?xml version='1.0' encoding='utf-8'?>
<Project xmlns='http://schemas.microsoft.com/developer/msbuild/2003'>
    <PropertyGroup>
        {0}
    </PropertyGroup>
</Project>", string.Join(Environment.NewLine + "        ", propertyLines)));
        }

        private static XName ScopedName(string localName)
        {
            return XNamespace.Get(MSBuildNamespace) + localName;
        }
    }
}