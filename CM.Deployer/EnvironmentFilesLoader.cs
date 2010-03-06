using System;
using System.Collections.Generic;
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

        public string[] GetEnvironments()
        {
            var files = fileSystem.ListAllFilesIn(environmentsDirectory, "*" + configurationFileExtension);
            return files.Select(file => Path.GetFileNameWithoutExtension(file)).ToArray();
        }

        public IList<KeyValuePair<string, string>> GetProperties(string environment)
        {
            var path = string.Format(@"{0}\{1}{2}", environmentsDirectory, environment, configurationFileExtension);
            return LoadProperties(path);
        }

        public IList<KeyValuePair<string, string>> LoadProperties(string path)
        {
            var xml = XElement.Parse(fileSystem.ReadAllText(path));
            return xml.Descendants(ScopedName("PropertyGroup")).Descendants()
                .Select(node => new KeyValuePair<string, string>(node.Name.LocalName, node.Value)).ToArray();
        }

        public void SaveProperties(IList<KeyValuePair<string, string>> properties, string path)
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