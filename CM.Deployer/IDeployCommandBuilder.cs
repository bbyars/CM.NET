using System.Collections.Generic;

namespace CM.Deployer
{
    public interface IDeployCommandBuilder
    {
        void SetEnvironmentProperties(IList<KeyValuePair<string, string>> properties);
        string CommandLine { get; }
    }
}