using System.Collections.Generic;

namespace CM.Deployer
{
    public interface IEnvironmentLoader
    {
        string[] GetEnvironments();
        IList<KeyValuePair<string, string>> GetProperties(string environment);
        IList<KeyValuePair<string, string>> LoadProperties(string path);
        void SaveProperties(IList<KeyValuePair<string, string>> properties, string path);
    }
}