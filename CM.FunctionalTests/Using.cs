using System;
using System.IO;
using CM.Deploy.UI;

namespace CM.FunctionalTests
{
    public static class Using
    {
        public static void Directory(string directoryName, Action test)
        {
            if (System.IO.Directory.Exists(directoryName))
                System.IO.Directory.Delete(directoryName, true);
            System.IO.Directory.CreateDirectory(directoryName);

            var originalDirectory = Environment.CurrentDirectory;
            Environment.CurrentDirectory = directoryName;
            try
            {
                test();
            }
            finally
            {
                Environment.CurrentDirectory = originalDirectory;

                // I can't figure out why we can't delete the directory using Directory.Delete, but we can in the shell.
                // Neither Process Explorer nor Unlocker find any locks on the directory.
                new ProcessRunner("cmd").Run(string.Format("/c rmdir /S /Q \"{0}\"", directoryName), TimeSpan.FromSeconds(10));
            }
        }

        public static void SvnRepo(Action<string> test)
        {
            var repoPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            System.IO.Directory.CreateDirectory(repoPath);
            try
            {
                var runner = new ProcessRunner("svnadmin");
                runner.Run(string.Format("create \"{0}\"", repoPath), TimeSpan.FromSeconds(3));
                NUnit.Framework.Assert.That(runner.WasSuccessful, runner.StandardError);

                var repoUrl = "file:///" + repoPath.Replace('\\', '/');
                test(repoUrl);
            }
            finally
            {
                // I can't figure out why we can't delete the directory using Directory.Delete, but we can in the shell.
                // Neither Process Explorer nor Unlocker find any locks on the directory.
                new ProcessRunner("cmd").Run(string.Format("/c rmdir /S /Q \"{0}\"", repoPath), TimeSpan.FromSeconds(10));
            }
        }
    }
}
