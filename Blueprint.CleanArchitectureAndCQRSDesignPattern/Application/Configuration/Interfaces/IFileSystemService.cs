using System.IO;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Interfaces
{
    public interface IFileSystemService
    {
        void Delete(string path);
        FileStream Create(string path);
        void Move(string sourceFileName, string destinationFileName);
        bool Exists(string path);
        DirectoryInfo CreateDirectory(string path);
    }
}
