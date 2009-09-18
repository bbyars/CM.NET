using System;
using System.Diagnostics;

namespace CM.Deploy.UI
{
    public class ProcessRunner
    {
        public delegate void OutputUpdatedHandler();

        public event OutputUpdatedHandler OutputUpdated;

        private readonly string command;

        public ProcessRunner(string command)
        {
            this.command = command;
            ExitCode = -1;
        }

        public virtual int Pid { get; private set; }
        public virtual string StandardOutput { get; private set; }
        public virtual int ExitCode { get; private set; }

        public virtual bool WasSuccessful
        {
            get { return ExitCode == 0; }
        }
        
        public virtual void Run(string args, TimeSpan timeout)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = command, 
                Arguments = args,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            var process = Process.Start(startInfo);
            Pid = process.Id;
            process.OutputDataReceived += OnStandardOutputUpdated;
            process.BeginOutputReadLine();

            process.WaitForExit(GetMillisecondsToWait(timeout));

            if (process.HasExited)
                ExitCode = process.ExitCode;
            else
                KillTree();
        }

        protected virtual void OnOutputUpdated()
        {
            var handler = OutputUpdated;
            if (handler != null) handler();
        }

        private static int GetMillisecondsToWait(TimeSpan timeout)
        {
            var milliseconds = Convert.ToInt64(timeout.TotalMilliseconds);
            return milliseconds > int.MaxValue ? int.MaxValue : Convert.ToInt32(milliseconds);
        }

        private void KillTree()
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "taskkill",
                Arguments = string.Format("/PID {0} /T /F", Pid),
                UseShellExecute = false,
                CreateNoWindow = true
            };
            var killProcess = Process.Start(startInfo);
            killProcess.WaitForExit(5000);
        }

        private void OnStandardOutputUpdated(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;

            if (!string.IsNullOrEmpty(StandardOutput))
                StandardOutput += Environment.NewLine;

            StandardOutput += e.Data;
            OnOutputUpdated();
        }
    }
}