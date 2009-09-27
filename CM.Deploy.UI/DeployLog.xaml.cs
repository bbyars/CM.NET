using System;
using System.IO;
using System.Windows;
using CM.Common;
using Microsoft.Win32;

namespace CM.Deploy.UI
{
    public partial class DeployLog
    {
        public DeployLog(ProcessRunner processRunner)
        {
            InitializeComponent();
            processRunner.OutputUpdated += (() => Dispatcher.BeginInvoke(
                (Action)(() => uxLog.Text = Header(processRunner) + processRunner.StandardOutput)));
        }

        private static string Header(ProcessRunner processRunner)
        {
            return "Working Directory: " + processRunner.WorkingDirectory + Environment.NewLine
                + "Command: " + processRunner.CommandLine + Environment.NewLine + Environment.NewLine;
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Title = "Save Log File", CheckFileExists = false };
            if (dialog.ShowDialog().GetValueOrDefault())
                File.WriteAllText(dialog.FileName, uxLog.Text);
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Close();   
        }
    }
}
