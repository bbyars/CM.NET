using System;

namespace CM.Common
{
    public static class Shell
    {
        public static string MSBuild(string projectFileAndArgs, TimeSpan timeout)
        {
            var commandLine = string.Format(@"C:\Windows\Microsoft.NET\Framework\v3.5\MSBuild.exe {0}", projectFileAndArgs);
            var process = new ProcessRunner().Exec(commandLine, timeout);
            ThrowIfFailed(process);

            return process.StandardOutput;
        }

        public static void RmDir(string directoryName)
        {
            // I can't figure out why we can't delete the directory using Directory.Delete, but we can in the shell.
            // Neither Process Explorer nor Unlocker find any locks on the directory.
            // It only happens with svn files.
            var commandLine = string.Format("cmd /c rmdir /S /Q \"{0}\"", directoryName);
            var process = new ProcessRunner().Exec(commandLine, TimeSpan.FromSeconds(10));
            ThrowIfFailed(process);
        }

        private static void ThrowIfFailed(SystemProcess process)
        {
            if (process.WasSuccessful)
                return;

            throw new ApplicationException(process.ToString());
        }
    }
}