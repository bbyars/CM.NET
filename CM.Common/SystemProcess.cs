using System;
using System.Diagnostics;

namespace CM.Common
{
    public class SystemProcess
    {
        public event DataReceivedEventHandler OutputUpdated;
        public event DataReceivedEventHandler ErrorUpdated;

        private readonly Process process;

        public SystemProcess(Process process)
        {
            this.process = process;
            process.OutputDataReceived += (sender, e) => Receive(e, () => StandardOutput, s => StandardOutput += s, OutputUpdated);
            process.BeginOutputReadLine();
            process.ErrorDataReceived += (sender, e) => Receive(e, () => StandardError, s => StandardError += s, ErrorUpdated);
            process.BeginErrorReadLine();
        }

        public virtual string CommandLine
        {
            get { return String.Format("{0} {1}", process.StartInfo.FileName, process.StartInfo.Arguments).Trim(); }
        }

        public virtual string WorkingDirectory
        {
            get { return process.StartInfo.WorkingDirectory; }
        }

        public virtual int Pid
        {
            get { return process.Id; }
        }

        public virtual string StandardOutput { get; private set; }
        public virtual string StandardError { get; private set; }

        public virtual int ExitCode
        {
            get { return process.ExitCode; }
        }

        public virtual bool WasSuccessful
        {
            get { return ExitCode == 0; }
        }

        public virtual bool WasKilled { get; private set; }

        public virtual bool HasExited
        {
            get { return process.HasExited; }
        }

        public virtual void KillTree()
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "taskkill",
                Arguments = String.Format("/PID {0} /T /F", Pid),
                UseShellExecute = false,
                CreateNoWindow = true
            };
            var killProcess = Process.Start(startInfo);
            killProcess.WaitForExit(30000);
            WasKilled = true;
        }

        public virtual void WaitForExit(TimeSpan timeout)
        {
            process.WaitForExit(GetMillisecondsToWait(timeout));
        }

        public override string ToString()
        {
            if (!HasExited)
                return string.Format("<{0}>", CommandLine);

            if (WasKilled)
                return string.Format("<{0}> timed out", CommandLine);

            if (!WasSuccessful)
                return string.Format("<{0}> failed with exit code {1}\n\tstdout: {2}\n\tstderr: {3}",
                    CommandLine, ExitCode, StandardOutput, StandardError);

            return string.Format("<{0}> succeeded", CommandLine);
        }

        private static int GetMillisecondsToWait(TimeSpan timeout)
        {
            var milliseconds = Convert.ToInt64(timeout.TotalMilliseconds);
            return milliseconds > Int32.MaxValue ? Int32.MaxValue : Convert.ToInt32(milliseconds);
        }

        private void Receive(DataReceivedEventArgs e, Func<string> current, Action<string> append, DataReceivedEventHandler eventHandler)
        {
            if (e.Data == null)
                return;

            if (!string.IsNullOrEmpty(current()))
                append(Environment.NewLine);

            append(e.Data);

            var handler = eventHandler;
            if (handler != null) handler(this, e);
        }
    }
}