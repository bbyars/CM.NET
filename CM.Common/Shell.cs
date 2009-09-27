using System;

namespace CM.Common
{
    public static class Shell
    {
        public static string MSBuild(string projectFile, TimeSpan timeout)
        {
            var runner = new ProcessRunner(@"C:\Windows\Microsoft.NET\Framework\v3.5\MSBuild.exe");
            runner.Run(projectFile, timeout);
            ThrowIfFailed(runner);

            return runner.StandardOutput;
        }

        public static void RmDir(string directoryName)
        {
            // I can't figure out why we can't delete the directory using Directory.Delete, but we can in the shell.
            // Neither Process Explorer nor Unlocker find any locks on the directory.
            // It only happens with svn files.
            var runner = new ProcessRunner("cmd");
            runner.Run(string.Format("/c rmdir /S /Q \"{0}\"", directoryName), TimeSpan.FromSeconds(10));
            ThrowIfFailed(runner);
        }

        private static void ThrowIfFailed(ProcessRunner runner)
        {
            if (!runner.WasSuccessful)
                throw new ApplicationException(string.Format(
                    runner.Command + " failed:\nstdout: {0}\nstderr: {1}", runner.StandardOutput, runner.StandardError));
        }
    }
}