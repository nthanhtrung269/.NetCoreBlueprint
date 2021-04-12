using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Commands;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.DeleteFile
{
    public class ResizeImageCommand : CommandBase<bool>
    {
        public long ImageId { set; get; }
        public string Query { set; get; }

        public ResizeImageCommand(long id, string query)
        {
            ImageId = id;
            Query = query;
        }
    }
}
