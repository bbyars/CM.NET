using System.Collections.Generic;
using CM.Common;

namespace CM.Deployer
{
    public interface IDeployView
    {
        string SelectedEnvironment { get; }
        void ShowEnvironments(string[] environments);
        string ExternalFile { get; }

        bool UsePackagedEnvironment { get; }
        bool EnvironmentEnabled { get; set; }
        bool ExternalFileEnabled { get; set; }

        void ShowProperties(IDictionary<string, string> properties);

        void ShowLogView(SystemProcess process);
    }
}
