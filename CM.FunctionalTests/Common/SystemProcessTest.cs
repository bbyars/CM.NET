using System;
using System.IO;
using System.Text;
using CM.Common;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace CM.FunctionalTests.Common
{
    [TestFixture]
    public class SystemProcessTest
    {
        [Test]
        public void ShouldAllowAsynchronousReadingOfStandardOutput()
        {
            var output = "";
            var process = new ProcessRunner().Start("cmd /c echo test");
            process.OutputUpdated += () => output = process.StandardOutput;
            process.WaitForExit(TimeSpan.FromSeconds(30));
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

        [Test]
        public void ShouldAllowAsynchronousReadingOfStandardError()
        {
            var error = "";
            var process = new ProcessRunner().Start("svn");
            process.ErrorUpdated += () => error = process.StandardError;
            process.WaitForExit(TimeSpan.FromSeconds(30));
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
        public void ShouldAllowKillingTreeWhenRunInBackground()
        {
            SystemAssert.AssertProcessKilled("ping", () =>
            {
                var process = new ProcessRunner().Start("cmd /c cmd /c ping 127.0.0.1 -n 20");
                Assert.That(process.WasKilled, Is.False);
                process.KillTree();
                Assert.That(process.WasKilled, Is.True);
            });
        }

        [Test]
        public void ShouldReportCommandLineBeforeProcessExits()
        {
            var process = new ProcessRunner().Start("ping 127.0.0.1 -n 20");
            Assert.That(process.ToString(), Is.EqualTo("<ping 127.0.0.1 -n 20>"));
            process.KillTree();
        }

        [Test]
        public void ShouldReportTimeoutWhenKilled()
        {
            var process = new ProcessRunner().Start("ping 127.0.0.1 -n 20");
            process.KillTree();
            Assert.That(process.ToString(), Is.EqualTo("<ping 127.0.0.1 -n 20> timed out"));
        }

        [Test]
        public void ShouldReportFailureWithStandardOutputAndStandardError()
        {
            var process = new ProcessRunner().Exec("svn", TimeSpan.FromSeconds(30));
            Assert.That(process.ToString(), Is.EqualTo("<svn> failed with exit code 1\n\tstdout: \n\tstderr: Type 'svn help' for usage."));
        }

        [Test]
        public void ShouldReportSuccess()
        {
            var process = new ProcessRunner().Exec("cmd /c echo test", TimeSpan.FromSeconds(30));
            Assert.That(process.ToString(), Is.EqualTo("<cmd /c echo test> succeeded"));
        }
    }
}