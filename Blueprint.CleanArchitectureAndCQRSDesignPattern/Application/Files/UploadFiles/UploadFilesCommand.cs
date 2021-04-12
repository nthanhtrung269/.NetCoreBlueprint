using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Commands;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.UploadFiles
{
    public class UploadFilesCommand : CommandBase<UploadResponse>
    {
        public UploadResponse UploadResponse { get; }

        public UploadFilesCommand(UploadResponse uploadResponse)
        {
            UploadResponse = uploadResponse;
        }
    }
}
