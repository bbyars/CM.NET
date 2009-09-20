using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
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
                try
                {
//                    System.IO.Directory.Delete(directoryName, true);
                    var runner = new ProcessRunner("rmdir");
                    runner.Run(string.Format("/S /Q \"{0}\"", directoryName), TimeSpan.MaxValue);
                    Console.WriteLine("rmdir output: " + runner.StandardOutput);
                    Console.WriteLine("rmdir err: " + runner.StandardError);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public static void SvnRepo(Action<string> test)
        {
            //TODO: I want to use a relative directory under the test directory, but I 
            // can't figure out why we can't delete the directory here, but we can in the shell.
            // Neither Process Explorer nor Unlocker find any locks on the directory.
            // I'm providing an ugly work-around - writing to a temp directory and just leaving them there.
            var repoPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var currentDirectory = Environment.CurrentDirectory;
            Using.Directory(repoPath, () =>
            {
                var runner = new ProcessRunner("svnadmin");
                runner.Run("create .", TimeSpan.FromSeconds(3));
                NUnit.Framework.Assert.That(runner.WasSuccessful, runner.StandardError);

                var repoUrl = "file:///" + repoPath.Replace('\\', '/');
                Environment.CurrentDirectory = currentDirectory;
                test(repoUrl);
            });
        }
    }
}
