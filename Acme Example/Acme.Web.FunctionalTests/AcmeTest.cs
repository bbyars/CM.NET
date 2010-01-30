using System.IO;
using System.Net;
using Acme.Web.FunctionalTests.Properties;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Acme.Web.FunctionalTests
{
    [TestFixture]
    public class AcmeTest
    {
        [Test]
        public void SiteShouldRespond()
        {
            var response = GetResponse(Settings.Default.AcmeUrl);
            Assert.That(response, Text.Contains("Hello, World!"));
        }

        private static string GetResponse(string url)
        {
            var request = WebRequest.Create(url);
            var response = (HttpWebResponse)request.GetResponse();
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }
    }
}