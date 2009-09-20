using CM.Common;
using NUnit.Framework;

namespace CM.UnitTests.Common
{
    [TestFixture]
    public class NetworkDirectoryTest
    {
        [Test]
        public void NetworkPathConvertsRemoteMachineToUncPath()
        {
            var directory = new NetworkDirectory("host", @"C:\dir\subdir");
            Assert.AreEqual(@"\\host\C$\dir\subdir", directory.NetworkPath);
        }

        [Test]
        public void NetworkPathShouldNotChangeUncPaths()
        {
            var directory = new NetworkDirectory("localhost", @"\\host\share\dir");
            Assert.AreEqual(@"\\host\share\dir", directory.NetworkPath);
        }

        [Test]
        public void NetworkPathKeepsLocalhostPaths()
        {
            var directory = new NetworkDirectory("localhost", @"C:\dir\subdir");
            Assert.AreEqual(@"C:\dir\subdir", directory.NetworkPath);
        }
    }
}