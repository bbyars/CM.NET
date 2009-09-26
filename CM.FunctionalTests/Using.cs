using System;
using System.IO;
using CM.Common;

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
                Shell.RmDir(directoryName);
            }
        }

        public static void SvnRepo(Action<string> test)
        {
            var repoPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var currentDirectory = Environment.CurrentDirectory;
            Using.Directory(repoPath, () =>
            {
                var runner = new ProcessRunner("svnadmin");
                runner.Run("create .", TimeSpan.FromSeconds(10));
                NUnit.Framework.Assert.That(runner.WasSuccessful,
                    "svnadmin failed\n\tstdout: {0}\n\tstderr: {1}", runner.StandardOutput, runner.StandardError);

                Using.Directory(Path.Combine(currentDirectory, "svn"), () => test(MakeFileUrl(repoPath)));
            });
        }

        public static void GitRepo(Action<string> test)
        {
            var repoPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var currentDirectory = Environment.CurrentDirectory;
            Using.Directory(repoPath, () =>
            {
                var runner = new ProcessRunner("git");
                runner.Run("init", TimeSpan.FromSeconds(10));
                NUnit.Framework.Assert.That(runner.WasSuccessful,
                    "git init failed\n\tstdout: {0}\n\tstderr: {1}", runner.StandardOutput, runner.StandardError);

                File.WriteAllText(Path.Combine(repoPath, "git-repo"), "");
                runner.Run("add git-repo", TimeSpan.FromSeconds(5));
                NUnit.Framework.Assert.That(runner.WasSuccessful,
                    "git add failed\n\tstdout: {0}\n\tstderr: {1}", runner.StandardOutput, runner.StandardError);

                runner.Run("commit -a -m 'init'", TimeSpan.FromSeconds(5));
                NUnit.Framework.Assert.That(runner.WasSuccessful,
                    "git commit failed\n\tstdout: {0}\n\tstderr: {1}", runner.StandardOutput, runner.StandardError);

                Using.Directory(Path.Combine(currentDirectory, "git"), () => test(repoPath));
            });
        }

        private static string MakeFileUrl(string repoPath)
        {
            return "file:///" + repoPath.Replace('\\', '/').Replace(" ", "%20");
        }
    }
}
