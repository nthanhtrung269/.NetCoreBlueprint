using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Interfaces
{
    public interface IImageService
    {
        /// <summary>
        /// Open a stream to read image
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        FileStream ReadImage(string path);
        /// <summary>
        /// Resize Image & save
        /// </summary>
        /// <param name="image"></param>
        /// <param name="ratio"></param>
        /// <param name="fileExtension"></param>
        /// <param name="outputStream"></param>
        /// <returns></returns>
        Task<(int newHeight, int newWidth)> ResizeImage(Stream image, (int? width, int? height) ratio, string fileExtension, FileStream outputStream);
        
        /// <summary>
        /// Can resize original to this request? 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="original"></param>
        /// <returns></returns>
        bool CanResize((int? width, int? height) request, (int? width, int? height) original);

        /// <summary>
        /// Check should resize image base on original image, request ratio & app config
        /// </summary>
        /// <param name="originalRatio"></param>
        /// <returns></returns>
        bool ShouldResize((int width, int height) originalRatio, (int? width, int? height) requestRatio);
        Task<(int height, int width)> GetImageDimentions(Stream stream, string fileExtension, CancellationToken cancellationToken);
    }
}
