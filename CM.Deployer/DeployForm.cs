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

        public string SelectedEnvironment
        {
            get { return uxEnvironment.SelectedValue.ToString(); }
        }

        public void ShowEnvironments(string[] environments)
        {
            uxEnvironment.Items.Clear();
            foreach (var environment in environments)
                uxEnvironment.Items.Add(environment);
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
            get { return uxEnvironment.Enabled; }
            set { uxEnvironment.Enabled = value; }
        }

        public bool ExternalFileEnabled
        {
            get { return uxExternalFile.Enabled; }
            set { uxExternalFile.Enabled = uxLoadExternalFile.Enabled = value; }
        }

        public void ShowProperties(IDictionary<string, string> properties)
        {
            uxProperties.Items.Clear();
            foreach (var property in properties)
                uxProperties.Items.Add(new ListViewItem(property.Key, property.Value));
        }

        public void ShowLogView(SystemProcess process)
        {
            var logForm = new DeployLog(process);
            logForm.Show();
        }

        private void ClickRadio(object sender, System.EventArgs e)
        {
            presenter.ToggleConfigSelection();
        }

        private void LoadExternalFile(object sender, System.EventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = "Config Files|*.properties", Title = "Open config file" };
            if (dialog.ShowDialog() == DialogResult.OK)
                ExternalFile = dialog.FileName;
        }

        private void Save(object sender, System.EventArgs e)
        {
        }

        private void Deploy(object sender, System.EventArgs e)
        {
            presenter.Deploy();
        }

        private void EnvironmentSelected(object sender, System.EventArgs e)
        {
            presenter.LoadEnvironment(SelectedEnvironment);
        }
    }
}
