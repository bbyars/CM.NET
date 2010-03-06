using System.Collections.Generic;
using CM.Common;

namespace CM.Deployer
{
    public interface IDeployView
    {
        string SelectedEnvironment { get; }
        void ShowEnvironments(string[] environments);
        string ExternalFile { get; }
        PropertyList Properties { get; set; }
        bool UsePackagedEnvironment { get; }
        bool EnvironmentEnabled { get; set; }
        bool ExternalFileEnabled { get; set; }
        void ShowLogView(SystemProcess process);
    }
}
