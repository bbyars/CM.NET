using System;
using System.IO;
using System.Windows.Forms;
using CM.Common;

namespace CM.Deployer
{
    public partial class DeployLog : Form
    {
        private readonly SystemProcess process;

        public DeployLog(SystemProcess process)
        {
            InitializeComponent();
            this.process = process;
            process.OutputUpdated += UpdateLog;
            process.ErrorUpdated += UpdateLog;
        }

        private void UpdateLog()
        {
            Invoke((Action)(() =>
                uxLog.Text = Header + process.StandardOutput + Environment.NewLine + Environment.NewLine + process.StandardError));
        }

        private string Header
        {
            get
            {
                return "Working Directory: " + process.WorkingDirectory + Environment.NewLine
                       + "Command: " + process.CommandLine + Environment.NewLine + Environment.NewLine;
            }
        }

        private void Save(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Save Log File",
                CheckFileExists = false,
                InitialDirectory = Environment.CurrentDirectory
            };
            if (dialog.ShowDialog() == DialogResult.OK)
                File.WriteAllText(dialog.FileName, uxLog.Text);
        }

        private void ShowWorkingDirectory(object sender, EventArgs e)
        {
            new ProcessRunner().Start(string.Format("cmd /c explorer \"{0}\"", process.WorkingDirectory));
        }

        private void Kill(object sender, EventArgs e)
        {
            process.KillTree();
        }
    }
}
