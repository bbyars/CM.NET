using System.Collections.Generic;

namespace CM.Deployer
{
    public interface IEnvironmentLoader
    {
        string[] GetEnvironments();
        IList<KeyValuePair<string, string>> GetProperties(string environment);
    }
}