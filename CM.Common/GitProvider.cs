using System;
using System.IO;

namespace CM.Common
{
    public class GitProvider : ISourceControlProvider, IDisposable
    {
        private readonly ILogger log;
        private readonly string localPath;
        private readonly TimeSpan commandTimeout;

        public static GitProvider Clone(string url, ILogger log, TimeSpan commandTimeout)
        {
            var localPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(localPath);

            var runner = new ProcessRunner(localPath);
            var process = runner.Exec(string.Format("git clone \"{0}\"", url), TimeSpan.FromMinutes(5));
            if (!process.WasSuccessful)
                throw new ArgumentException(process.ToString());

            return new GitProvider(log, localPath, commandTimeout);
        }

        private GitProvider(ILogger log, string localPath, TimeSpan commandTimeout)
        {
            this.log = log;
            this.localPath = localPath;
            this.commandTimeout = commandTimeout;
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

        private void RunCommand(string command, string workingDirectory, Action<SystemProcess> onFailure)
        {
            log.Info("git {0}", command);
            var processRunner = new ProcessRunner(workingDirectory);
            var process = processRunner.Exec("git " + command, commandTimeout);
            if (!process.WasSuccessful)
                onFailure(process);
        }

        private void LogFailure(SystemProcess process)
        {
            log.Error(process.ToString());
            throw new ApplicationException("git command failed");
        }

        public void Dispose()
        {
            Directory.Delete(localPath, true);
        }
    }
}
