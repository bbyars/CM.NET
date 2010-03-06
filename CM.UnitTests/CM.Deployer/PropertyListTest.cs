using CM.Deployer;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace CM.UnitTests.CM.Deployer
{
    [TestFixture]
    public class PropertyListTest
    {
        [Test]
        public void EmptyPropertyListsShouldBeEqual()
        {
            Assert.That(new PropertyList(), Is.EqualTo(new PropertyList()));
        }

        [Test]
        public void FilledPropertyListsShouldBeEqual()
        {
            var first = new PropertyList().Add("key1", "value1").Add("key2", "value2");
            var second = new PropertyList().Add("key1", "value1").Add("key2", "value2");
            Assert.That(first, Is.EqualTo(second));
        }

        [Test]
        public void PropertyListsInDifferentOrderAreNotEqual()
        {
            var first = new PropertyList().Add("key1", "value1").Add("key2", "value2");
            var second = new PropertyList().Add("key2", "value2").Add("key1", "value1");
            Assert.That(first, Is.Not.EqualTo(second));
        }
    }
}