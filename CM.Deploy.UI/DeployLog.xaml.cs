using System;
using System.IO;
using System.Windows;
using CM.Common;
using Microsoft.Win32;

namespace CM.Deploy.UI
{
    public partial class DeployLog
    {
        public DeployLog(SystemProcess process)
        {
            InitializeComponent();
            process.OutputUpdated += (() => Dispatcher.BeginInvoke(
                (Action)(() => uxLog.Text = Header(process) + process.StandardOutput)));
        }

        private static string Header(SystemProcess process)
        {
            return "Working Directory: " + process.WorkingDirectory + Environment.NewLine
                + "Command: " + process.CommandLine + Environment.NewLine + Environment.NewLine;
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Save Log File",
                CheckFileExists = false,
                InitialDirectory = Environment.CurrentDirectory
            };
            if (dialog.ShowDialog().GetValueOrDefault())
                File.WriteAllText(dialog.FileName, uxLog.Text);
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Close();   
        }
    }
}
