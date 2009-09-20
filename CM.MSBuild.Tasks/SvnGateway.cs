using System;
using System.Diagnostics;

namespace CM.MSBuild.Tasks
{
    public class SvnGateway
    {
        public virtual bool Exists(string url)
        {
            var process = RunCommand(string.Format("ls \"{0}\"", url));
            return process.ExitCode == 0;
        }

        public virtual void CreateWorkingDirectory(string url, string localPath)
        {
            RunCommand(string.Format("co \"{0}\" \"{1}\"", url, localPath));
        }

        public virtual void Commit(string workingDirectory)
        {
            
        }

        public virtual void Import(string workingDirectory, string url, string message)
        {
            RunCommand(string.Format("import \"{0}\" \"{1}\" --message \"{2}\"", workingDirectory, url, message));
        }

        public virtual void Branch(string sourceUrl, string destinationUrl)
        {
            
        }

        public virtual void AddFile(string file, string workingDirectory)
        {
            throw new NotImplementedException();
        }

        public virtual void AddDirectory(string directory, string workingDirectory)
        {
            throw new NotImplementedException();
        }

        public virtual void UpdateFile(string file, string workingDirectory)
        {
            throw new NotImplementedException();
        }

        public virtual void DeleteFile(string file, string workingDirectory)
        {
            throw new NotImplementedException();
        }

        public virtual void DeleteDirectory(string directory, string workingDirectory)
        {
            throw new NotImplementedException();
        }

        private Process RunCommand(string command)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "svn",
                Arguments = command,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            Console.WriteLine("svn " + command);
            var process = Process.Start(startInfo);
            process.WaitForExit();
            Console.WriteLine("  Output: " + process.StandardOutput.ReadToEnd());
            Console.WriteLine("  Error: " + process.StandardError.ReadToEnd());
            return process;
        }
    }
}