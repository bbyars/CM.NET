using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace CM.Common
{
    public class ProcessRunner
    {
        public delegate void UpdatedHandler();

        public event UpdatedHandler OutputUpdated;
        public event UpdatedHandler ErrorUpdated;

        public ProcessRunner()
        {
        }

        public ProcessRunner(string command)
        {
            Command = command;
            WorkingDirectory = Environment.CurrentDirectory;
            ExitCode = -1;
        }

        public virtual string Command { get; private set; }
        public virtual string CommandLine { get; private set; }
        public virtual string WorkingDirectory { get; set; }
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
            CommandLine = string.Format("{0} {1}", Command, args);
            var startInfo = new ProcessStartInfo
            {
                FileName = Command,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = WorkingDirectory
            };

            var process = Process.Start(startInfo);
            Pid = process.Id;
            process.OutputDataReceived += OnStandardOutputUpdated;
            process.BeginOutputReadLine();
            process.ErrorDataReceived += OnStandardErrorUpdated;
            process.BeginErrorReadLine();

            return process;
        }

        public virtual CMProcess Exec(string command, TimeSpan timeout)
        {
            var match = Regex.Match(command, @"^([^ ]+)(.*)$");
            Command = match.Groups[1].Value;
            var process = Start(match.Groups[2].Value);
            process.WaitForExit(GetMillisecondsToWait(timeout));

            if (process.HasExited)
                ExitCode = process.ExitCode;
            else
                KillTree();

            var cmProcess = new CMProcess();
            cmProcess.ExitCode = ExitCode;
            cmProcess.WasSuccessful = WasSuccessful;
            cmProcess.StandardOutput = StandardOutput;
            return cmProcess;
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

        public class CMProcess
        {
            public virtual int ExitCode { get; set; }
            public virtual bool WasSuccessful { get; set; }
            public virtual string StandardOutput { get; set; }
        }
    }
}