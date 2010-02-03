using System.IO;
using NUnit.Framework;

namespace CM.FunctionalTests.scripts
{
    public class CMTest
    {
        [SetUp]
        public void PrepareCMDirectory()
        {
            var cmFiles = new[] { "CM.Common.dll", "CM.MSBuild.Tasks.dll", "deployer.exe" };
            foreach (var file in cmFiles)
                File.Copy(file, Path.Combine("CM.NET", file), true);

            File.Copy(@"..\..\..\CM.Deployer\App.config", @"CM.NET\deployer.exe.config", true);
        }
    }
}