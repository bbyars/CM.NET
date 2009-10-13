using System;
using System.Diagnostics;
using System.IO;

namespace CM.Common
{
    public class GitProvider : ISourceControlProvider, IDisposable
    {
        private readonly ILogger log;
        private readonly string localPath;

        public static GitProvider Clone(string url, ILogger log)
        {
            var localPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(localPath);

            var runner = new ProcessRunner(localPath);
            var process = runner.Exec(string.Format("git clone \"{0}\"", url), TimeSpan.FromMinutes(5));
            if (!process.WasSuccessful)
                throw new ArgumentException(process.StandardError);

            return new GitProvider(log, localPath);
        }

        private GitProvider(ILogger log, string localPath)
        {
            this.log = log;
            this.localPath = localPath;
        }

        public virtual string[] MetadataDirectories
        {
            get { return new[] {".git"}; }
        }

        public virtual bool Exists(string path)
        {
            var result = true;
            RunCommand(string.Format("show \"{0}\"", path), ".", p => result = false);
            return result;
        }

        public virtual void CreateWorkingDirectory(string url, string localPath)
        {
            throw new NotImplementedException();
        }

        public virtual void Commit(string workingDirectory, string message)
        {
            throw new NotImplementedException();
        }

        public virtual void Import(string workingDirectory, string url, string message)
        {
            throw new NotImplementedException();
        }

        public virtual void Branch(string sourceUrl, string destinationUrl, string message)
        {
            throw new NotImplementedException();
        }

        public virtual void AddFile(string file, string workingDirectory)
        {
            RunCommand(string.Format("add \"{0}\"", file), ".", LogFailure);
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

        private void RunCommand(string command, string workingDirectory, Action<Process> onFailure)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = command,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = workingDirectory
            };
            log.Info("git {0}", command);
            var process = Process.Start(startInfo);
            process.WaitForExit();

            if (process.ExitCode != 0)
                onFailure(process);
        }

        private void LogFailure(Process process)
        {
            var stdout = process.StandardOutput.ReadToEnd();
            var stderr = process.StandardError.ReadToEnd();
            if (!string.IsNullOrEmpty(stdout))
                log.Error(stdout);
            if (!string.IsNullOrEmpty(stderr))
                log.Error(stderr);
        }

        public void Dispose()
        {
            Directory.Delete(localPath, true);
        }
    }
}
