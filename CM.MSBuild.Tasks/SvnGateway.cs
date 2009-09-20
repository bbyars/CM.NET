using System.Diagnostics;

namespace CM.MSBuild.Tasks
{
    public class SvnGateway
    {
        public virtual bool Exists(string url)
        {
            return RunCommand(string.Format("ls \"{0}\"", url), ".") == 0;
        }

        public virtual void CreateWorkingDirectory(string url, string localPath)
        {
            RunCommand(string.Format("checkout \"{0}\" \"{1}\"", url, localPath), ".");
        }

        public virtual void Commit(string workingDirectory, string message)
        {
            RunCommand(string.Format("commit . --message \"{0}\"", message), workingDirectory);
        }

        public virtual void Import(string workingDirectory, string url, string message)
        {
            RunCommand(string.Format("import \"{0}\" \"{1}\" --message \"{2}\"", workingDirectory, url, message), ".");
        }

        public virtual void Branch(string sourceUrl, string destinationUrl, string message)
        {
            RunCommand(string.Format("copy \"{0}\" \"{1}\" --message \"{2}\"", sourceUrl, destinationUrl, message), ".");
        }

        public virtual void AddFile(string file, string workingDirectory)
        {
            RunCommand(string.Format("add \"{0}\"", file), workingDirectory);
        }

        public virtual void AddDirectory(string directory, string workingDirectory)
        {
            RunCommand(string.Format("add \"{0}\"", directory), workingDirectory);
        }

        public virtual void UpdateFile(string file, string workingDirectory)
        {
        }

        public virtual void DeleteFile(string file, string workingDirectory)
        {
            RunCommand(string.Format("rm \"{0}\"", file), workingDirectory);
        }

        public virtual void DeleteDirectory(string directory, string workingDirectory)
        {
            RunCommand(string.Format("rm \"{0}\"", directory), workingDirectory);
        }

        private static int RunCommand(string command, string workingDirectory)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "svn",
                Arguments = command,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = workingDirectory
            };
            var process = Process.Start(startInfo);
            process.WaitForExit();
            return process.ExitCode;
        }
    }
}