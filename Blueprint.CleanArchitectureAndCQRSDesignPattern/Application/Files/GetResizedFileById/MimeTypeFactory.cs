namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.GetResizedFileById
{
    public class MimeTypeFactory
    {
        public string GetMimeType(string fileExtension)
        {
            if (fileExtension == ".bmp")
                return "image/bmp";

            if (fileExtension == ".apng")
                return "image/apng";

            if (fileExtension == ".gif")
                return "image/gif";

            if (fileExtension == ".icon" || fileExtension == ".cur")
                return "x-ico";

            if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".jfif" || fileExtension == ".pjpeg" || fileExtension == ".pjp")
                return "image/jpeg";

            if (fileExtension == ".png")
                return "image/png";

            if (fileExtension == ".svg")
                return "image/svg+xml";

            if (fileExtension == ".tif" || fileExtension == ".tiff")
                return "image/tiff";

            if (fileExtension == ".webp")
                return "image/webp";

            return "application/octet";
        }
    }
}
