using System;

namespace CM.FunctionalTests
{
    public static class Using
    {
        public static void Directory(string directoryName, Action test)
        {
            if (!System.IO.Directory.Exists(directoryName))
                System.IO.Directory.CreateDirectory(directoryName);

            var originalDirectory = Environment.CurrentDirectory;
            Environment.CurrentDirectory = directoryName;
            try
            {
                test();
            }
            finally
            {
                Environment.CurrentDirectory = originalDirectory;
                //System.IO.Directory.Delete(directoryName, true);
            }
        }
    }
}
