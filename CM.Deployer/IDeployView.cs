using System.Collections.Generic;
using CM.Common;

namespace CM.Deployer
{
    public interface IDeployView
    {
        string SelectedEnvironment { get; }
        void ShowEnvironments(string[] environments);
        string ExternalFile { get; }
        IList<KeyValuePair<string, string>> Properties { get; set; }
        bool UsePackagedEnvironment { get; }
        bool EnvironmentEnabled { get; set; }
        bool ExternalFileEnabled { get; set; }
        void ShowLogView(SystemProcess process);
    }
}
