using System;
using System.Diagnostics;
using System.IO;
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
            var runner = new ProcessRunner();
            var process = runner.Exec("cmd /c echo test", TimeSpan.MaxValue);
            Assert.That(process.WasSuccessful);
            Assert.That(process.ExitCode, Is.EqualTo(0));
            Assert.That(process.StandardOutput, Is.EqualTo("test"));
        }

        [Test]
        public void ShouldAllowAsynchronousReadingOfStandardOutput()
        {
            var runner = new ProcessRunner();
            var output = "";
            runner.OutputUpdated += () => output = runner.StandardOutput;
            runner.Exec("cmd /c echo test", TimeSpan.MaxValue);
            Assert.That(output, Is.EqualTo("test"));
        }

        [Test]
        public void ShouldRunGivenProcessAndSaveStandardError()
        {
            // I had trouble finding a simple Windows exe (in system32) that 
            // behaves correctly with regards to stderr
            var runner = new ProcessRunner();
            runner.Exec("svn", TimeSpan.MaxValue);
            Assert.That(!runner.WasSuccessful);
            Assert.That(runner.StandardError, Is.EqualTo("Type 'svn help' for usage."));
        }

        [Test]
        public void ShouldAllowAsynchronousReadingOfStandardError()
        {
            var runner = new ProcessRunner();
            var error = "";
            runner.ErrorUpdated += () => error = runner.StandardError;
            runner.Exec("svn", TimeSpan.MaxValue);
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
                runner.Exec("cmd /c type multiline.txt", TimeSpan.MaxValue);
                Assert.That(runner.StandardOutput, Is.EqualTo(lines.ToString()));
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
                var runner = new ProcessRunner("cmd");
                runner.Start("/c cmd /c ping 127.0.0.1 -n 20");
                runner.KillTree();
            });
        }

        private static void AssertProcessKilled(string processName, Action test)
        {
            var processesBeforeTest = Process.GetProcessesByName(processName).Length;
            test();
            var processesAfterTest = Process.GetProcessesByName(processName).Length;
            Assert.That(processesAfterTest, Is.EqualTo(processesBeforeTest));
        }
    }
}