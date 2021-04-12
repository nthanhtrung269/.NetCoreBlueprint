using Microsoft.Extensions.Options;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.ApplicationSettings;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Interfaces;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Services
{
    public class ImageService : IImageService
    {
        private readonly IImageResizerService _imageResizer;
        private readonly AssetSettings _assetSetting;

        public ImageService(IImageResizerService imageResizer, IOptions<AssetSettings> assetSetting)
        {
            _imageResizer = imageResizer;
            _assetSetting = assetSetting.Value;
        }

        public bool CanResize((int? width, int? height) request, (int? width, int? height) original)
        {
            if (request.width != null && request.width > 0 && request.height != null && request.height > 0)
                return (decimal)request.width / request.height == (decimal)original.width / original.height;

            return true;
        }

        public FileStream ReadImage(string path)
        {
            if (File.Exists(path))
            {
                var fs = File.OpenRead(path);
                return fs;
            }

            return null;
        }

        public async Task<(int newHeight, int newWidth)> ResizeImage(Stream image, (int? width, int? height) ratio, string fileExtension, FileStream outputStream)
        {
            return await _imageResizer.Resize(image, ratio, fileExtension, outputStream);
        }

        /// <summary>
        /// Gets image dimentions.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="fileExtention">The fileExtention.</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <returns>(int height, int width).</returns>
        public async Task<(int height, int width)> GetImageDimentions(Stream stream, string fileExtension, CancellationToken cancellationToken)
        {
            return await _imageResizer.GetImageDimentionsAsync(stream, fileExtension, cancellationToken);
        }

        public bool ShouldResize((int width, int height) originalRatio, (int? width, int? height) requestRatio)
        {
            return (!requestRatio.width.HasValue || originalRatio.width > requestRatio.width)
                && (!requestRatio.height.HasValue || originalRatio.height > requestRatio.height)
                && (requestRatio.width.HasValue || requestRatio.height.HasValue);
        }
    }
}
