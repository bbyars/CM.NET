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

            Load += (sender, e) => Initialize();
            uxUseExternalFile.CheckedChanged += (sender, e) => ToggleConfigSelection();
            uxUsePackagedFile.CheckedChanged += (sender, e) => ToggleConfigSelection();
            uxEnvironments.SelectedValueChanged += (sender, e) => ResetProperties();
            uxLoadExternalFile.Click += (sender, e) => SelectFile("Open Config File", LoadExternalFile);
            uxSave.Click += (sender, e) => SelectFile("Save Config File", Save);
            uxDeploy.Click += (sender, e) => ShowDeployLog(Deploy());
        }

        public virtual void Initialize()
        {
            Environments = environmentLoader.GetEnvironments();
            UsePackagedEnvironment = true;
            ToggleConfigSelection();
        }

        public virtual void ToggleConfigSelection()
        {
            EnvironmentEnabled = UsePackagedEnvironment;
            ExternalFileEnabled = !UsePackagedEnvironment;
        }

        public virtual void ResetProperties()
        {
            Properties = environmentLoader.GetProperties(SelectedEnvironment);
        }

        public virtual void LoadExternalFile(string path)
        {
            ExternalFile = path;
            Properties = environmentLoader.LoadProperties(path);
        }

        public virtual void Save(string path)
        {
            environmentLoader.SaveProperties(Properties, path);
        }

        public virtual SystemProcess Deploy()
        {
            commandBuilder.SetEnvironmentProperties(Properties);
            return processRunner.Start(commandBuilder.CommandLine);
        }

        public virtual string SelectedEnvironment
        {
            get { return uxEnvironments.SelectedItem.ToString(); }
            set { uxEnvironments.SelectedItem = value; }
        }

        public virtual string ExternalFile
        {
            get { return uxExternalFile.Text; }
            set { uxExternalFile.Text = value; }
        }

        public virtual bool UsePackagedEnvironment
        {
            get { return uxUsePackagedFile.Checked; }
            set { uxUsePackagedFile.Checked = value; }
        }

        public virtual bool EnvironmentEnabled
        {
            get { return uxEnvironments.Enabled; }
            set { uxEnvironments.Enabled = value; }
        }

        public virtual bool ExternalFileEnabled
        {
            get { return uxExternalFile.Enabled; }
            set { uxExternalFile.Enabled = uxLoadExternalFile.Enabled = value; }
        }

        public virtual PropertyList Properties
        {
            get
            {
                var properties = new PropertyList();
                foreach (DataGridViewRow row in uxProperties.Rows)
                    if (row.Cells[0].Value != null)
                        properties.Add(row.Cells[0].Value.ToString(), row.Cells[1].Value.ToString());
                return properties;
            }
            set
            {
                uxProperties.Rows.Clear();
                foreach (var property in value)
                    uxProperties.Rows.Add(property.Key, property.Value);
            }
        }

        public virtual string[] Environments
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

        private static void SelectFile(string dialogTitle, Action<string> continuation)
        {
            var dialog = new OpenFileDialog { Filter = "Config Files|*" + Settings.Default.ConfigurationFileExtension, Title = dialogTitle };
            if (dialog.ShowDialog() == DialogResult.OK)
                continuation(dialog.FileName);
        }

        private static void ShowDeployLog(SystemProcess process)
        {
            var logForm = new DeployLog(process);
            logForm.Show();
        }
    }
}
