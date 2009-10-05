namespace CM.Common
{
    public interface ISourceControlProvider
    {
        string[] MetadataDirectories { get; }
        bool Exists(string path);
        void CreateWorkingDirectory(string url, string localPath);
        void Commit(string workingDirectory, string message);
        void Import(string workingDirectory, string url, string message);
        void Branch(string sourceUrl, string destinationUrl, string message);
        void AddFile(string file, string workingDirectory);
        void AddDirectory(string directory, string workingDirectory);
        void UpdateFile(string file, string workingDirectory);
        void DeleteFile(string file, string workingDirectory);
        void DeleteDirectory(string directory, string workingDirectory);
    }
}