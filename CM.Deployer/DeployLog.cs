using System;
using System.Drawing;
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
            this.process = process;

            InitializeComponent();

            Load += (sender, e) => Initialize();
            process.OutputUpdated += (sender, e) => AppendOutput(e.Data);
            process.ErrorUpdated += (sender, e) => AppendError(e.Data);
            uxShowWorkingDirectory.Click += (sender, e) => ShowWorkingDirectory();
            uxKill.Click += (sender, e) => Kill();
            uxSave.Click += (sender, e) => SelectFile(Save);
        }

        public virtual string Log
        {
            get { return uxLog.Text; }
        }

        public virtual void Initialize()
        {
            AppendOutput(Header);
        }

        public virtual void AppendOutput(string text)
        {
            Invoke((Action)(() => uxLog.Text += text));
        }

        public virtual void AppendError(string text)
        {
            Invoke((Action)(() =>
            {
                uxLog.ForeColor = Color.Red;
                uxLog.Text += "<error>" + text + "******************";
                uxLog.ForeColor = Color.White;
            }));
        }

        public virtual void Save(string path)
        {
            File.WriteAllText(path, Log);
        }

        public virtual void ShowWorkingDirectory()
        {
            new ProcessRunner().Start(string.Format("cmd /c explorer \"{0}\"", process.WorkingDirectory));
        }

        public virtual void Kill()
        {
            process.KillTree();
        }

        private string Header
        {
            get
            {
                return "Working Directory: " + process.WorkingDirectory + Environment.NewLine
                       + "Command: " + process.CommandLine + Environment.NewLine + Environment.NewLine;
            }
        }

        private static void SelectFile(Action<string> continuation)
        {
            var dialog = new SaveFileDialog
            {
                Title = "Save Log File",
                CheckFileExists = false,
                InitialDirectory = Environment.CurrentDirectory
            };

            if (dialog.ShowDialog() == DialogResult.OK)
                continuation(dialog.FileName);
        }
    }
}
