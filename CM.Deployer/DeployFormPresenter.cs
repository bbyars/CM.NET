using System;
using System.IO;
using CM.Common;
using CM.Deployer.Properties;

namespace CM.Deployer
{
    public class DeployFormPresenter
    {
        private readonly IDeployView view;
        private readonly ProcessRunner processRunner;
        private readonly IEnvironmentLoader environmentLoader;
        private readonly Settings settings = Settings.Default;

        public DeployFormPresenter(IDeployView view, ProcessRunner processRunner, IEnvironmentLoader environmentLoader)
        {
            this.view = view;
            this.processRunner = processRunner;
            this.environmentLoader = environmentLoader;
        }

        public virtual void Initialize()
        {
            view.ShowEnvironments(environmentLoader.GetEnvironments());
            ToggleConfigSelection();
        }

        public virtual void ToggleConfigSelection()
        {
            view.EnvironmentEnabled = view.UsePackagedEnvironment;
            view.ExternalFileEnabled = !view.UsePackagedEnvironment;
        }

        public virtual void LoadEnvironment(string environment)
        {
            view.ShowProperties(environmentLoader.GetProperties(environment));
        }

        public virtual void Deploy()
        {
            var commandLine = string.Format(
                @"C:\Windows\Microsoft.NET\Framework\v3.5\msbuild.exe {0} /t:Deploy /p:""ConfigPath={1}"" /p:""PackageDirectory=.""", 
                settings.MSBuildFilename, ConfigFilePath);
            view.ShowLogView(processRunner.Start(commandLine));
        }

        private string ConfigFilePath
        {
            get
            {
                if (view.UsePackagedEnvironment)
                    return Path.Combine(Path.Combine(Environment.CurrentDirectory, settings.EnvironmentsDirectory),
                        view.SelectedEnvironment + settings.ConfigurationFileExtension);
                else
                    return view.ExternalFile;
            }
        }
    }
}
