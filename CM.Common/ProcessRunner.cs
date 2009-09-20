using System;
using System.Diagnostics;

namespace CM.Common
{
    public class ProcessRunner
    {
        public delegate void UpdatedHandler();

        public event UpdatedHandler OutputUpdated;
        public event UpdatedHandler ErrorUpdated;

        private readonly string command;

        public ProcessRunner(string command)
        {
            this.command = command;
            ExitCode = -1;
        }

        public virtual int Pid { get; private set; }
        public virtual string StandardOutput { get; private set; }
        public virtual string StandardError { get; private set; }
        public virtual int ExitCode { get; private set; }

        public virtual bool WasSuccessful
        {
            get { return ExitCode == 0; }
        }
        
        public virtual Process Start(string args)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = command,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            var process = Process.Start(startInfo);
            Pid = process.Id;
            process.OutputDataReceived += OnStandardOutputUpdated;
            process.BeginOutputReadLine();
            process.ErrorDataReceived += OnStandardErrorUpdated;
            process.BeginErrorReadLine();

            return process;
        }

        public virtual void Run(string args, TimeSpan timeout)
        {
            var process = Start(args);
            process.WaitForExit(GetMillisecondsToWait(timeout));

            if (process.HasExited)
                ExitCode = process.ExitCode;
            else
                KillTree();
        }

        public virtual void KillTree()
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

        protected virtual void OnOutputUpdated()
        {
            var handler = OutputUpdated;
            if (handler != null) handler();
        }

        protected virtual void OnErrorUpdated()
        {
            var handler = ErrorUpdated;
            if (handler != null) handler();
        }

        private static int GetMillisecondsToWait(TimeSpan timeout)
        {
            var milliseconds = Convert.ToInt64(timeout.TotalMilliseconds);
            return milliseconds > int.MaxValue ? int.MaxValue : Convert.ToInt32(milliseconds);
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

        private void OnStandardErrorUpdated(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;

            if (!string.IsNullOrEmpty(StandardError))
                StandardError += Environment.NewLine;

            StandardError += e.Data;
            OnErrorUpdated();
        }
    }
}