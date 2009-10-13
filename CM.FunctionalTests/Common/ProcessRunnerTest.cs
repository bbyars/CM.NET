using System;
using System.IO;
using System.Linq;
using System.Text;
using CM.Common;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace CM.FunctionalTests.Common
{
    [TestFixture]
    public class ProcessRunnerTest
    {
        [Test]
        public void ShouldRunGivenProcessAndSaveStandardOutput()
        {
            var process = new ProcessRunner().Exec("cmd /c echo test", TimeSpan.MaxValue);
            Assert.That(process.WasSuccessful);
            Assert.That(process.ExitCode, Is.EqualTo(0));
            Assert.That(process.StandardOutput, Is.EqualTo("test"));
        }

        [Test, Ignore]
        public void ShouldAllowAsynchronousReadingOfStandardOutput()
        {
            var output = "";
            var process = new ProcessRunner().Start("cmd /c echo test");
            process.OutputUpdated += () => output = process.StandardOutput;
            Assert.That(output, Is.EqualTo("test"));
        }

        [Test]
        public void ShouldRunGivenProcessAndSaveStandardError()
        {
            // I had trouble finding a simple Windows exe (in system32) that 
            // behaves correctly with regards to stderr
            var process = new ProcessRunner().Exec("svn", TimeSpan.MaxValue);
            Assert.That(!process.WasSuccessful);
            Assert.That(process.StandardError, Is.EqualTo("Type 'svn help' for usage."));
        }

        [Test, Ignore]
        public void ShouldAllowAsynchronousReadingOfStandardError()
        {
            var error = "";
            var process = new ProcessRunner().Start("svn");
            process.ErrorUpdated += () => error = process.StandardError;
            Assert.That(error, Is.EqualTo("Type 'svn help' for usage."));
        }

        [Test]
        public void ShouldHandleMultilineOutput()
        {
            Using.Directory("processRunnerTest", () =>
            {
                var lines = new StringBuilder().AppendLine("first line").Append("second line");
                File.WriteAllText("multiline.txt", lines.ToString());
                var runner = new ProcessRunner();
                var process = runner.Exec("cmd /c type multiline.txt", TimeSpan.MaxValue);
                Assert.That(process.StandardOutput, Is.EqualTo(lines.ToString()));
            });
        }

        [Test]
        public void ShouldKillProcessAfterTimeout()
        {
            AssertProcessKilled("ping", () =>
            {
                var runner = new ProcessRunner();
                var start = DateTime.Now;
                runner.Exec("ping 127.0.0.1 -n 20", TimeSpan.FromMilliseconds(100));
                Assert.That(DateTime.Now - start, Is.LessThan(TimeSpan.FromSeconds(5)));
            });
        }

        [Test]
        public void ShouldKillEntireProcessTreeAfterTimeout()
        {
            AssertProcessKilled("ping", () =>
            {
                var runner = new ProcessRunner();
                runner.Exec("cmd /c cmd /c ping 127.0.0.1 -n 20", TimeSpan.FromMilliseconds(100));
            });
        }

        [Test]
        public void ShouldAllowKillingTreeWhenRunInBackground()
        {
            AssertProcessKilled("ping", () =>
            {
                var runner = new ProcessRunner();
                var process = runner.Start("cmd /c cmd /c ping 127.0.0.1 -n 20");
                process.KillTree();
            });
        }

        private static void AssertProcessKilled(string processName, Action test)
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