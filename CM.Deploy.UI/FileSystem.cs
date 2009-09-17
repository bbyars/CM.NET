using System.IO;
using System.Linq;

namespace CM.Deploy.UI
{
    public class FileSystem
    {
        public virtual string[] ListAllFilesIn(string directory)
        {
            return Directory.GetFiles(directory).Select(file => Path.GetFileName(file)).ToArray();
        }

        public virtual string ReadAllText(string filename)
        {
            return File.ReadAllText(filename);
        }
    }
}