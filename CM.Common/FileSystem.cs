using System.IO;
using System.Linq;

namespace CM.Common
{
    public class FileSystem
    {
        public virtual string[] ListAllFilesIn(string directory, string searchPattern)
        {
            return Directory.GetFiles(directory, searchPattern).Select(file => Path.GetFileName(file)).ToArray();
        }

        public virtual string ReadAllText(string filename)
        {
            return File.ReadAllText(filename);
        }
    }
}