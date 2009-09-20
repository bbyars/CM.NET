using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using CM.Deploy.UI;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace CM.FunctionalTests.Deploy.UI
{
    [TestFixture]
    public class ProcessRunnerTest
    {
        [Test]
        public void ShouldNotReportSuccessBeforeRunning()
        {
            Assert.That(new ProcessRunner("cmd").WasSuccessful, Is.False);
        }

        [Test]
        public void ShouldRunGivenProcessAndSaveStandardOutput()
        {
            var runner = new ProcessRunner("cmd");
            runner.Run("/c echo test", TimeSpan.MaxValue);
            Assert.That(runner.WasSuccessful);
            Assert.That(runner.ExitCode, Is.EqualTo(0));
            Assert.That(runner.StandardOutput, Is.EqualTo("test"));
        }

        [Test]
        public void ShouldAllowAsynchronousReadingOfStandardOutput()
        {
            var runner = new ProcessRunner("cmd");
            var output = "";
            runner.OutputUpdated += () => output = runner.StandardOutput;
            runner.Run("/c echo test", TimeSpan.MaxValue);
            Assert.That(output, Is.EqualTo("test"));
        }

        [Test]
        public void ShouldRunGivenProcessAndSaveStandardError()
        {
            // I had trouble finding k simple Windows exe (in system32) that 
            // behaves correctly with regards to stderr
            var runner = new ProcessRunner("svn");
            runner.Run("", TimeSpan.MaxValue);
            Console.WriteLine(runner.StandardOutput);
            Assert.That(!runner.WasSuccessful);
            Assert.That(runner.StandardError, Is.EqualTo("Type 'svn help' for usage."));
        }

        [Test]
        public void ShouldAllowAsynchronousReadingOfStandardError()
        {
            var runner = new ProcessRunner("svn");
            var error = "";
            runner.ErrorUpdated += () => error = runner.StandardError;
            runner.Run("", TimeSpan.MaxValue);
            Assert.That(error, Is.EqualTo("Type 'svn help' for usage."));
        }

        [Test]
        public void ShouldHandleMultilineOutput()
        {
            Using.Directory("processRunnerTest", () =>
            {
                var lines = new StringBuilder().AppendLine("first line").Append("second line");
                File.WriteAllText("multiline.txt", lines.ToString());
                var runner = new ProcessRunner("cmd");
                runner.Run("/c type multiline.txt", TimeSpan.MaxValue);
                Assert.That(runner.StandardOutput, Is.EqualTo(lines.ToString()));
            });
        }

        [Test]
        public void ShouldKillProcessAfterTimeout()
        {
            var runner = new ProcessRunner("ping");
            var start = DateTime.Now;
            runner.Run("127.0.0.1 -n 20", TimeSpan.FromMilliseconds(100));
            Assert.That(DateTime.Now - start, Is.LessThan(TimeSpan.FromSeconds(3)));

            var orphanedProcesses = Process.GetProcessesByName("ping");
            Assert.That(orphanedProcesses.Length, Is.EqualTo(0), "did not kill process");
        }

        [Test]
        public void ShouldKillEntireProcessTreeAfterTimeout()
        {
            var runner = new ProcessRunner("cmd");
            runner.Run("/c cmd /c ping 127.0.0.1 -n 20", TimeSpan.FromSeconds(1));
            var orphanedProcesses = Process.GetProcessesByName("ping");
            Assert.That(orphanedProcesses.Length, Is.EqualTo(0));
        }

        [Test]
        public void ShouldAllowKillingTreeWhenRunInBackground()
        {
            var runner = new ProcessRunner("cmd");
            runner.Start("/c cmd /c ping 127.0.0.1 -n 20");
            runner.KillTree();
            var orphanedProcesses = Process.GetProcessesByName("ping");
            Assert.That(orphanedProcesses.Length, Is.EqualTo(0));
        }
    }
}