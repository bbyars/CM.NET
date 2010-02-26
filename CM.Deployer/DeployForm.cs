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

        public void LoadForm(object sender, EventArgs e)
        {
            Environments = environmentLoader.GetEnvironments();
            UsePackagedEnvironment = true;
            ToggleConfigSelection();
        }

        public void ClickRadio(object sender, EventArgs e)
        {
            ToggleConfigSelection();
        }

        public void EnvironmentSelected(object sender, EventArgs e)
        {
            Properties = environmentLoader.GetProperties(SelectedEnvironment);
        }

        public void LoadExternalFile(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = "Config Files|*" + Settings.Default.ConfigurationFileExtension, Title = "Open config file" };
            if (dialog.ShowDialog() == DialogResult.OK)
                ExternalFile = dialog.FileName;
        }

        public void Save(object sender, EventArgs e)
        {
        }

        public void Deploy(object sender, EventArgs e)
        {
            commandBuilder.SetEnvironmentProperties(Properties);
            var logForm = new DeployLog(processRunner.Start(commandBuilder.CommandLine));
            logForm.Show();
        }

        public string SelectedEnvironment
        {
            get { return uxEnvironments.SelectedItem.ToString(); }
            set { uxEnvironments.SelectedItem = value; }
        }

        public string ExternalFile
        {
            get { return uxExternalFile.Text; }
            set { uxExternalFile.Text = value; }
        }

        public bool UsePackagedEnvironment
        {
            get { return uxUsePackagedFile.Checked; }
            set { uxUsePackagedFile.Checked = value; }
        }

        public bool EnvironmentEnabled
        {
            get { return uxEnvironments.Enabled; }
            set { uxEnvironments.Enabled = value; }
        }

        public bool ExternalFileEnabled
        {
            get { return uxExternalFile.Enabled; }
            set { uxExternalFile.Enabled = uxLoadExternalFile.Enabled = value; }
        }

        public IList<KeyValuePair<string, string>> Properties
        {
            get
            {
                var properties = new List<KeyValuePair<string, string>>();
                foreach (DataGridViewRow row in uxProperties.Rows)
                    if (row.Cells[0].Value != null)
                        properties.Add(new KeyValuePair<string, string>(row.Cells[0].Value.ToString(), row.Cells[1].Value.ToString()));
                return properties;
            }
            set
            {
                uxProperties.Rows.Clear();
                foreach (var property in value)
                    uxProperties.Rows.Add(property.Key, property.Value);
            }
        }

        public string[] Environments
        {
            get
            {
                var result = new List<string>();
                foreach (var item in uxEnvironments.Items)
                    result.Add(item.ToString());
                return result.ToArray();
            }
            set
            {
                uxEnvironments.Items.Clear();
                foreach (var environment in value)
                    uxEnvironments.Items.Add(environment);
            }
        }

        private void ToggleConfigSelection()
        {
            EnvironmentEnabled = UsePackagedEnvironment;
            ExternalFileEnabled = !UsePackagedEnvironment;
        }
    }
}
