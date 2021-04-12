using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Interfaces;
using System.IO;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Infrastructure.Services
{
    public class FileSystemService : IFileSystemService
    {
        public void Delete(string path)
        {
            File.Delete(path);
        }

        public FileStream Create(string path)
        {
            return File.Create(path);
        }

        public void Move(string sourceFileName, string destinationFileName)
        {
            File.Move(sourceFileName, destinationFileName);
        }

        public bool Exists(string path)
        {
            return Directory.Exists(path);
        }

        public DirectoryInfo CreateDirectory(string path)
        {
            return Directory.CreateDirectory(path);
        }
    }
}
