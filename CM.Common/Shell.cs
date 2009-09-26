using System;

namespace CM.Common
{
    public static class Shell
    {
        public static string RunMSBuild(string projectFile, TimeSpan timeout)
        {
            var runner = new ProcessRunner(@"C:\Windows\Microsoft.NET\Framework\v3.5\MSBuild.exe");
            runner.Run(projectFile, timeout);
            if (!runner.WasSuccessful)
                throw new ApplicationException(string.Format(
                    "MSBuild failed:\nstdout: {0}\nstderr: {1}", runner.StandardOutput, runner.StandardError));

            return runner.StandardOutput;
        }

        public static void RmDir(string directoryName)
        {
            // I can't figure out why we can't delete the directory using Directory.Delete, but we can in the shell.
            // Neither Process Explorer nor Unlocker find any locks on the directory.
            // It only happens with svn files.
            new ProcessRunner("cmd").Run(string.Format("/c rmdir /S /Q \"{0}\"", directoryName), TimeSpan.FromSeconds(10));
        }
    }
}