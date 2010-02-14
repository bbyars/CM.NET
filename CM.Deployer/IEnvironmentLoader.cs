using System.Collections.Generic;

namespace CM.Deployer
{
    public interface IEnvironmentLoader
    {
        string[] GetEnvironments();
        IDictionary<string, string> GetProperties(string environment);

    }
}