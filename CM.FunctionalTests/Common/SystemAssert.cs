using System;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace CM.FunctionalTests.Common
{
    public static class SystemAssert
    {
        public static void AssertProcessKilled(string processName, Action test)
        {
            // In attempt to get rid of intermittent test failures, rather than comparing raw
            // process counts, we instead assert that there aren't any PIDs after the test runs
            // that didn't exist prior to the test run for the given process name.  This isn't
            // perfect - the process could have started concurrent with the test run but 
            // independent from it, but it is robust enough to avoid an intermittent failure
            // condition you get comparing counts because one of the processes already running
            // died while the test was executing.
            var pidsBeforeTest = System.Diagnostics.Process.GetProcessesByName(processName).Select(p => p.Id.ToString()).ToArray();
            test();
            var pidsAfterTest = System.Diagnostics.Process.GetProcessesByName(processName).Select(p => p.Id.ToString()).ToArray();
            var errorMessage = string.Format("Did not kill process {0}.  PIDs before the test: {1}; PIDs after the test: {2}",
                processName, string.Join(", ", pidsBeforeTest), string.Join(", ", pidsAfterTest));
            Assert.That(pidsAfterTest.Any(pid => !pidsBeforeTest.Contains(pid)), Is.False, errorMessage);
        }
    }
}
