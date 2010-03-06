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
            uxEnvironments.SelectedValueChanged += (sender, e) => LoadEnvironmentProperties();
            uxLoadExternalFile.Click += (sender, e) => SelectFile(new OpenFileDialog(), LoadExternalFile);
            uxSave.Click += (sender, e) => SelectFile(new SaveFileDialog(), Save);
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

            if (UsePackagedEnvironment)
                LoadEnvironmentProperties();
            else
                LoadExternalFile(ExternalFile);
        }

        public virtual void LoadEnvironmentProperties()
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
            get { return uxEnvironments.SelectedItem == null ? "" : uxEnvironments.SelectedItem.ToString(); }
            set { uxEnvironments.SelectedItem = value; }
        }

        public virtual string ExternalFile
        {
            get { return uxExternalFile.Text; }
            private set { uxExternalFile.Text = value; }
        }

        public virtual bool UsePackagedEnvironment
        {
            get { return uxUsePackagedFile.Checked; }
            set { uxUsePackagedFile.Checked = value; }
        }

        public virtual bool EnvironmentEnabled
        {
            get { return uxEnvironments.Enabled; }
            private set { uxEnvironments.Enabled = value; }
        }

        public virtual bool ExternalFileEnabled
        {
            get { return uxExternalFile.Enabled; }
            private set { uxExternalFile.Enabled = uxLoadExternalFile.Enabled = value; }
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
                if (value != null)
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

        private static void SelectFile(FileDialog dialog, Action<string> continuation)
        {
            dialog.Filter = "Config Files|*." + Settings.Default.ConfigurationFileExtension;
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
