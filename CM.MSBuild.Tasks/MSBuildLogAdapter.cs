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

        public void Info(string message, params object[] formatArgs)
        {
            msbuildLogger.LogMessage(message, formatArgs);
        }
    }
}