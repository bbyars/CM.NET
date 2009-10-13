using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace CM.Common
{
    public class ProcessRunner
    {
        private readonly Regex pattern = new Regex(@"^([^ ]+)(.*)$");

        private readonly string workingDirectory;

        public ProcessRunner() : this(Environment.CurrentDirectory)
        {
        }

        public ProcessRunner(string workingDirectory)
        {
            this.workingDirectory = workingDirectory;
        }

        public virtual SystemProcess Start(string commandLine)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = FileName(commandLine),
                Arguments = Arguments(commandLine),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = workingDirectory
            };

            var process = System.Diagnostics.Process.Start(startInfo);
            return new SystemProcess(process);
        }

        public virtual SystemProcess Exec(string commandLine, TimeSpan timeout)
        {
            var process = Start(commandLine);
            process.WaitForExit(timeout);

            if (!process.HasExited)
                process.KillTree();

            return process;
        }

        private string FileName(string commandLine)
        {
            return pattern.Match(commandLine).Groups[1].Value;
        }

        private string Arguments(string commandLine)
        {
            return pattern.Match(commandLine).Groups[2].Value;
        }
    }
}