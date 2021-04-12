using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Commands;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.DeleteFile
{
    public class DeleteFileCommand : CommandBase
    {
        public long FileId { get; }

        public DeleteFileCommand(long fileId)
        {
            FileId = fileId;
        }
    }
}
