using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Interfaces
{
    public interface IImageResizerService
    {
        Task<(int newHeight, int newWidth)> Resize(Stream stream, (int? width, int? height) ratio, string fileExtension, Stream outputStream);
        Task<(int height, int width)> GetImageDimentionsAsync(Stream stream, string fileExtention, CancellationToken cancellationToken);
    }
}
