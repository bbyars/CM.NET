using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using CM.Common;
using CM.Deploy.UI.Properties;

namespace CM.Deploy.UI
{
    public class DeployFormPresenter
    {
        private readonly IDeployView view;
        private readonly FileSystem fileSystem;
        private readonly ProcessRunner processRunner;
        private readonly Settings settings = Settings.Default;

        private const string MSBuildNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";

        public DeployFormPresenter(IDeployView view, FileSystem fileSystem, ProcessRunner processRunner)
        {
            this.view = view;
            this.fileSystem = fileSystem;
            this.processRunner = processRunner;
        }

        public virtual void Initialize()
        {
            var files = fileSystem.ListAllFilesIn(settings.EnvironmentsDirectory, "*" + settings.ConfigurationFileExtension);
            var environments = files.Select(file => Path.GetFileNameWithoutExtension(file)).ToArray();
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
            var path = string.Format(@"{0}\{1}{2}", settings.EnvironmentsDirectory, environment, settings.ConfigurationFileExtension);
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
            var args = string.Format("{0} /t:Deploy /p:\"ConfigPath={1}\" /p:\"PackageDirectory=.\"", 
                settings.MSBuildFilename, ConfigFilePath);
            view.ShowLogView(processRunner);
            processRunner.Run(args, TimeSpan.MaxValue);
        }

        private string ConfigFilePath
        {
            get
            {
                if (view.UsePackagedEnvironment)
                    return Path.Combine(Path.Combine(Environment.CurrentDirectory, settings.EnvironmentsDirectory),
                        view.SelectedEnvironment + settings.ConfigurationFileExtension);
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