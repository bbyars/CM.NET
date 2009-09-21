using CM.Common;
using Microsoft.Build.Utilities;

namespace CM.MSBuild.Tasks
{
    public class MSBuildLogAdapter : ILogger
    {
        private readonly TaskLoggingHelper msbuildLogger;

        public MSBuildLogAdapter(TaskLoggingHelper msbuildLogger)
        {
            this.msbuildLogger = msbuildLogger;
        }

        public bool HasErrors { get; private set; }

        public void Info(string message, params object[] formatArgs)
        {
            msbuildLogger.LogMessage(message, formatArgs);
        }

        public void Error(string message, params object[] formatArgs)
        {
            HasErrors = true;
            msbuildLogger.LogWarning(message, formatArgs);
        }
    }
}