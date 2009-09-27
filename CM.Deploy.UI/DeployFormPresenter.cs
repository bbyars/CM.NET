using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using CM.Common;

namespace CM.Deploy.UI
{
    public class DeployFormPresenter
    {
        private readonly IDeployView view;
        private readonly FileSystem fileSystem;
        private readonly ProcessRunner processRunner;

        private const string MSBuildNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";

        public DeployFormPresenter(IDeployView view, FileSystem fileSystem, ProcessRunner processRunner)
        {
            this.view = view;
            this.fileSystem = fileSystem;
            this.processRunner = processRunner;
        }

        public virtual void Initialize()
        {
            var environments = fileSystem.ListAllFilesIn("Environments", "*.properties")
                .Select(file => Path.GetFileNameWithoutExtension(file)).ToArray();
            view.ShowEnvironments(environments);
            ToggleConfigSelection();
        }

        public virtual void ToggleConfigSelection()
        {
            view.EnvironmentEnabled = view.UsePackagedEnvironment;
            view.ExternalFileEnabled = !view.UsePackagedEnvironment;
        }

        public virtual void LoadEnvironment(string environment)
        {
            var path = string.Format(@"Environments\{0}.properties", environment);
            var xml = XElement.Parse(fileSystem.ReadAllText(path));
            var keyValuePairs = xml.Descendants(ScopedName("PropertyGroup")).Descendants()
                .Select(node => new KeyValuePair<string, string>(node.Name.LocalName, node.Value)).ToArray();

            var properties = new Dictionary<string, string>();
            foreach (var pair in keyValuePairs)
                properties.Add(pair.Key, pair.Value);

            view.ShowProperties(properties);
        }

        public virtual void Deploy()
        {
            var msbuildFile = fileSystem.ListAllFilesIn(".", "*.proj")[0];
            var args = string.Format("{0} /t:Deploy /p:\"ConfigPath={1}\" /p:\"PackageDirectory=.\"", 
                msbuildFile, ConfigFilePath);
            view.ShowLogView(processRunner);
            processRunner.Run(args, TimeSpan.MaxValue);
        }

        private string ConfigFilePath
        {
            get
            {
                if (view.UsePackagedEnvironment)
                    return string.Format(@"Environments\{0}.properties", view.SelectedEnvironment);
                else
                    return view.ExternalFile;
            }
        }

        private static XName ScopedName(string localName)
        {
            return XNamespace.Get(MSBuildNamespace) + localName;
        }
    }
}