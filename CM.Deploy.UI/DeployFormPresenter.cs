using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace CM.Deploy.UI
{
    public class DeployFormPresenter
    {
        private readonly IDeployView view;
        private readonly FileSystem fileSystem;

        private const string MSBuildNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";

        public DeployFormPresenter(IDeployView view, FileSystem fileSystem)
        {
            this.view = view;
            this.fileSystem = fileSystem;
        }

        public virtual void Initialize()
        {
            view.ShowEnvironments(fileSystem.ListAllFilesIn("Environments")
                .Select(file => Path.GetFileNameWithoutExtension(file)).ToArray());
            ToggleConfigSelection();
        }

        public virtual void ToggleConfigSelection()
        {
            view.EnvironmentEnabled = view.UsePackagedEnvironment;
            view.ExternalFileEnabled = !view.UsePackagedEnvironment;
        }

        public void LoadEnvironment(string environment)
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

        public static XName ScopedName(string localName)
        {
            return XNamespace.Get(MSBuildNamespace) + localName;
        }
    }
}