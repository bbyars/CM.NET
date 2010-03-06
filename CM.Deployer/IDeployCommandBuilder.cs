namespace CM.Deployer
{
    public interface IDeployCommandBuilder
    {
        void SetEnvironmentProperties(PropertyList properties);
        string CommandLine { get; }
    }
}