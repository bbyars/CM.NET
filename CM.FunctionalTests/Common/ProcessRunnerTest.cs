using System;
using CM.Common;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace CM.FunctionalTests.Common
{
    [TestFixture]
    public class ProcessRunnerTest
    {
        [Test]
        public void ExecShouldSynchronouslyRunProcess()
        {
            var process = new ProcessRunner().Exec("cmd /c echo test", TimeSpan.MaxValue);
            Assert.That(process.WasSuccessful);
            Assert.That(process.StandardOutput, Is.EqualTo("test"));
        }

        [Test]
        public void ExecShouldKillProcessAfterTimeout()
        {
            SystemAssert.AssertProcessKilled("ping", () =>
            {
                var runner = new ProcessRunner();
                var start = DateTime.Now;
                runner.Exec("ping 127.0.0.1 -n 20", TimeSpan.FromMilliseconds(100));

                // Lots of buffer adding because of virtualization, etc
                // Should take 20 seconds or so without being killed
                Assert.That(DateTime.Now - start, Is.LessThan(TimeSpan.FromSeconds(8)));
            });
        }

        [Test]
        public void ExecShouldKillEntireProcessTreeAfterTimeout()
        {
            SystemAssert.AssertProcessKilled("ping", () =>
            {
                var runner = new ProcessRunner();
                runner.Exec("cmd /c cmd /c ping 127.0.0.1 -n 20", TimeSpan.FromMilliseconds(100));
            });
        }

        [Test]
        public void StartShouldRunProcessInBackground()
        {
            var runner = new ProcessRunner();
            var process = runner.Start("ping 127.0.0.1 -n 20");
            Assert.That(process.HasExited, Is.False);
            process.KillTree();
        }

        [Test]
        public void ShouldUseGivenWorkingDirectory()
        {
            var runner = new ProcessRunner(@"C:\");
            var process = runner.Exec("cmd /c cd", TimeSpan.FromSeconds(10));
            Assert.That(process.StandardOutput, Is.EqualTo(@"C:\"));
        }
    }
}