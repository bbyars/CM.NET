using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CM.Common;
using CM.Deployer.Properties;

namespace CM.Deployer
{
    public partial class DeployForm : Form
    {
        private readonly IEnvironmentLoader environmentLoader;
        private readonly IDeployCommandBuilder commandBuilder;
        private readonly ProcessRunner processRunner;

        public DeployForm() : this(
            new EnvironmentFilesLoader(new FileSystem(), Settings.Default.EnvironmentsDirectory, Settings.Default.ConfigurationFileExtension),
            new MSBuildCommandBuilder(new FileSystem(), @"C:\windows\Microsoft.NET\Framework\v3.5\MSBuild.exe", Settings.Default.MSBuildFilename, "Deploy"),
            new ProcessRunner())
        {
        }

        public DeployForm(IEnvironmentLoader environmentLoader, IDeployCommandBuilder commandBuilder, ProcessRunner processRunner)
        {
            this.environmentLoader = environmentLoader;
            this.commandBuilder = commandBuilder;
            this.processRunner = processRunner;
            InitializeComponent();
        }

        private void LoadForm(object sender, EventArgs e)
        {
            ShowEnvironments(environmentLoader.GetEnvironments());
            ToggleConfigSelection();
        }

        private void ClickRadio(object sender, EventArgs e)
        {
            ToggleConfigSelection();
        }

        private void EnvironmentSelected(object sender, EventArgs e)
        {
            Properties = environmentLoader.GetProperties(SelectedEnvironment);
        }

        private void LoadExternalFile(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = "Config Files|*" + Settings.Default.ConfigurationFileExtension, Title = "Open config file" };
            if (dialog.ShowDialog() == DialogResult.OK)
                ExternalFile = dialog.FileName;
        }

        private void Save(object sender, EventArgs e)
        {
        }

        private void Deploy(object sender, EventArgs e)
        {
            commandBuilder.SetEnvironmentProperties(Properties);
            var logForm = new DeployLog(processRunner.Start(commandBuilder.CommandLine));
            logForm.Show();
        }

        private string SelectedEnvironment
        {
            get { return uxEnvironments.SelectedItem.ToString(); }
        }

        private string ExternalFile
        {
            get { return uxExternalFile.Text; }
            set { uxExternalFile.Text = value; }
        }

        private bool UsePackagedEnvironment
        {
            get { return uxUsePackagedFile.Checked; }
        }

        private bool EnvironmentEnabled
        {
            get { return uxEnvironments.Enabled; }
            set { uxEnvironments.Enabled = value; }
        }

        private bool ExternalFileEnabled
        {
            get { return uxExternalFile.Enabled; }
            set { uxExternalFile.Enabled = uxLoadExternalFile.Enabled = value; }
        }

        private IList<KeyValuePair<string, string>> Properties
        {
            get
            {
                var properties = new List<KeyValuePair<string, string>>();
                foreach (DataGridViewRow row in uxProperties.Rows)
                    properties.Add(new KeyValuePair<string, string>(row.Cells[0].ToString(), row.Cells[1].ToString()));
                return properties;
            }
            set
            {
                uxProperties.Rows.Clear();
                foreach (var property in value)
                    uxProperties.Rows.Add(property.Key, property.Value);
            }
        }

        private void ToggleConfigSelection()
        {
            EnvironmentEnabled = UsePackagedEnvironment;
            ExternalFileEnabled = !UsePackagedEnvironment;
        }

        private void ShowEnvironments(string[] environments)
        {
            uxEnvironments.Items.Clear();
            foreach (var environment in environments)
                uxEnvironments.Items.Add(environment);
        }
    }
}
