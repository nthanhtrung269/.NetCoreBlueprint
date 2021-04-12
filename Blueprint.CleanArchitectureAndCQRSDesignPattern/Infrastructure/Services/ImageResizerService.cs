using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Infrastructure.Services
{
    public class ImageResizerService : IImageResizerService
    {
        /// <summary>
        /// Resizes the image.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="ratio">The ratio.</param>
        /// <param name="fileExtension">The fileExtension.</param>
        /// <param name="outputStream">The outputStream.</param>
        /// <returns>Task{(int newHeight, int newWidth)}.</returns>
        public async Task<(int newHeight, int newWidth)> Resize(Stream stream, (int? width, int? height) ratio, string fileExtension, Stream outputStream)
        {
            using (Image image = await Image.LoadAsync(stream))
            {
                using (Image copy = image.Clone(x => x.Resize(ratio.width ?? 0, ratio.height ?? 0)))
                {
                    ImageFormatManager ifm = Configuration.Default.ImageFormatsManager;
                    IImageFormat format = ifm.FindFormatByFileExtension(fileExtension);
                    IImageEncoder encoder = ifm.FindEncoder(format);

                    await copy.SaveAsync(outputStream, encoder);

                    return (copy.Width, copy.Height);
                }
            }
        }

        /// <summary>
        /// Gets image dimentions.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="fileExtention">The fileExtention.</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <returns>(int height, int width).</returns>
        public async Task<(int height, int width)> GetImageDimentionsAsync(Stream stream, string fileExtention, CancellationToken cancellationToken)
        {
            IImageDecoder imageDecoder;

            if (fileExtention == ".jpeg" || fileExtention == ".jpg")
            {
                imageDecoder = new SixLabors.ImageSharp.Formats.Jpeg.JpegDecoder();
            }
            else if (fileExtention == ".png")
            {
                imageDecoder = new SixLabors.ImageSharp.Formats.Png.PngDecoder();
            }
            else
            {
                imageDecoder = new SixLabors.ImageSharp.Formats.Bmp.BmpDecoder();
            }

            if (stream.Position == stream.Length) //Check this because if your image is a .png, it might just throw an error
            {
                stream.Position = stream.Seek(0, SeekOrigin.Begin);
            }

            Image<SixLabors.ImageSharp.PixelFormats.Rgba32> imageSharp = await imageDecoder.DecodeAsync<SixLabors.ImageSharp.PixelFormats.Rgba32>(Configuration.Default, stream, cancellationToken);

            if (imageSharp != null)
            {
                return (imageSharp.Height, imageSharp.Width);
            }

            return (0, 0);
        }
    }
}
