using System.Collections.Generic;

namespace CM.Deploy.UI
{
    public interface IDeployView
    {
        string SelectedEnvironment { get; }
        string[] Environments { get; set; }
        string ExternalFile { get; }

        bool UsePackagedEnvironment { get; }
        bool EnvironmentEnabled { get; set; }
        bool ExternalFileEnabled { get; set; }

        void ShowProperties(IDictionary<string, string> properties);
    }
}