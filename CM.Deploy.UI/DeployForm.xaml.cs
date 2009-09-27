using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using CM.Common;
using Microsoft.Win32;

namespace CM.Deploy.UI
{
    public partial class DeployForm : IDeployView
    {
        private readonly DeployFormPresenter presenter;

        public DeployForm()
        {
            InitializeComponent();
            presenter = new DeployFormPresenter(this, new FileSystem(), new ProcessRunner(@"C:\Windows\Microsoft.NET\Framework\v3.5\msbuild.exe"));
            presenter.Initialize();
        }

        public string SelectedEnvironment
        {
            get { return uxEnvironments.SelectedValue.ToString(); }
        }

        public void ShowEnvironments(string[] environments)
        {
            uxEnvironments.Items.Clear();
            foreach (var environment in environments)
                uxEnvironments.Items.Add(environment);
        }

        public string ExternalFile
        {
            get { return uxConfigFile.Text; }
            private set { uxConfigFile.Text = value; }
        }

        public bool UsePackagedEnvironment
        {
            get { return uxEnvironmentsRadio.IsChecked.GetValueOrDefault(); }
        }

        public bool EnvironmentEnabled
        {
            get { return uxEnvironments.IsEnabled; }
            set { uxEnvironments.IsEnabled = value; }
        }

        public bool ExternalFileEnabled
        {
            get { return uxConfigFile.IsEnabled; }
            set { uxConfigFile.IsEnabled = uxLoadFileButton.IsEnabled = value; }
        }

        public void ShowProperties(IDictionary<string, string> properties)
        {
            uxProperties.Items.Clear();
            foreach (var property in properties)
                uxProperties.Items.Add(property);
        }

        public void ShowLogView(ProcessRunner processRunner, params string[] initialText)
        {
            var logForm = new DeployLog(processRunner, initialText);
            logForm.Show();
        }

        private void ClickRadio(object sender, RoutedEventArgs e)
        {
            presenter.ToggleConfigSelection();
        }

        private void LoadConfigFile(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog {Filter = "Config Files|*.properties", Title = "Open config file"};
            if (dialog.ShowDialog().GetValueOrDefault())
                ExternalFile = dialog.FileName;
        }

        private void EnvironmentSelected(object sender, SelectionChangedEventArgs e)
        {
            presenter.LoadEnvironment(SelectedEnvironment);
        }

        private void Deploy(object sender, RoutedEventArgs e)
        {
            presenter.Deploy();
        }
    }
}
