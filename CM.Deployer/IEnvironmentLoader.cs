namespace CM.Deployer
{
    public interface IEnvironmentLoader
    {
        string[] GetEnvironments();
        PropertyList GetProperties(string environment);
        PropertyList LoadProperties(string path);
        void SaveProperties(PropertyList properties, string path);
    }
}