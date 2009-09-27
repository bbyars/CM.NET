using System.IO;
using System.Linq;

namespace CM.Common
{
    public class FileSystem
    {
        public virtual string[] ListAllFilesIn(string directory, string searchPattern)
        {
            try
            {
                return Directory.GetFiles(directory, searchPattern).Select(file => Path.GetFileName(file)).ToArray();
            }
            catch (DirectoryNotFoundException)
            {
                return new string[0];
            }
        }

        public virtual string ReadAllText(string filename)
        {
            return File.ReadAllText(filename);
        }
    }
}