using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CM.Common;

namespace CM.Deployer
{
    public partial class DeployForm : Form, IDeployView
    {
        private readonly DeployFormPresenter presenter;

        public DeployForm()
        {
            InitializeComponent();
            presenter = new DeployFormPresenter(this, new FileSystem(), new ProcessRunner());
            presenter.Initialize();
        }

        public void ShowEnvironments(string[] environments)
        {
            uxEnvironments.Items.Clear();
            foreach (var environment in environments)
                uxEnvironments.Items.Add(environment);
        }

        public string SelectedEnvironment
        {
            get { return uxEnvironments.SelectedItem.ToString(); }
        }

        public string ExternalFile
        {
            get { return uxExternalFile.Text; }
            private set { uxExternalFile.Text = value; }
        }

        public bool UsePackagedEnvironment
        {
            get { return uxUsePackagedFile.Checked; }
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

        public void ShowProperties(IDictionary<string, string> properties)
        {
            uxProperties.Rows.Clear();
            foreach (var property in properties)
                uxProperties.Rows.Add(property.Key, property.Value);
        }

        public void ShowLogView(SystemProcess process)
        {
            var logForm = new DeployLog(process);
            logForm.Show();
        }

        private void ClickRadio(object sender, EventArgs e)
        {
            presenter.ToggleConfigSelection();
        }

        private void EnvironmentSelected(object sender, EventArgs e)
        {
            presenter.LoadEnvironment(SelectedEnvironment);
        }

        private void LoadExternalFile(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = "Config Files|*.properties", Title = "Open config file" };
            if (dialog.ShowDialog() == DialogResult.OK)
                ExternalFile = dialog.FileName;
        }

        private void Save(object sender, EventArgs e)
        {
        }

        private void Deploy(object sender, EventArgs e)
        {
            presenter.Deploy();
        }
    }
}
