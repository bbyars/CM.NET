using System;

namespace CM.Common
{
    public static class Shell
    {
        public static string MSBuild(string projectFile, TimeSpan timeout)
        {
            var runner = new ProcessRunner();
            runner.Exec(string.Format(@"C:\Windows\Microsoft.NET\Framework\v3.5\MSBuild.exe {0}", projectFile), timeout);
            ThrowIfFailed(runner);

            return runner.StandardOutput;
        }

        public static void RmDir(string directoryName)
        {
            // I can't figure out why we can't delete the directory using Directory.Delete, but we can in the shell.
            // Neither Process Explorer nor Unlocker find any locks on the directory.
            // It only happens with svn files.
            var runner = new ProcessRunner();
            runner.Exec(string.Format("cmd /c rmdir /S /Q \"{0}\"", directoryName), TimeSpan.FromSeconds(10));
            ThrowIfFailed(runner);
        }

        private static void ThrowIfFailed(ProcessRunner runner)
        {
            if (runner.WasSuccessful)
                return;

            if (runner.ExitCode == -1)
                throw new ApplicationException(string.Format("{0} timed out", runner.CommandLine));

            throw new ApplicationException(string.Format(
                "{0} failed with exit code {1}:\nstdout: {2}\nstderr: {3}", 
                runner.Command, runner.ExitCode, runner.StandardOutput, runner.StandardError));
        }
    }
}