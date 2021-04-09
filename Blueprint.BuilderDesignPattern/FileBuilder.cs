using System.IO;
using System.Linq;

namespace Blueprint.BuilderDesignPattern
{
    public class FileBuilder
    {
        private ImageFile _file;

        public FileBuilder()
        {
            Reset();
        }

        public ImageFile Build()
        {
            var file = _file;
            Reset();

            return file;
        }

        public FileBuilder BuildCloudUrl(string cloudUrlBase, string name)
        {
            _file.CloudUrl = PathHelper.CombineUrl(cloudUrlBase, name);
            return this;
        }

        public FileBuilder BuildFilePath(string filePathBase, string name)
        {
            _file.FilePath = Path.Combine(filePathBase, name);
            return this;
        }

        public FileBuilder BuildFileInfo(ImageFile file)
        {
            _file.FileType = file.FileType;
            _file.Height = file.Height;
            _file.Width = file.Width;
            _file.OriginalFileName = file.OriginalFileName;
            _file.Source = file.Source;
            _file.OriginalId = file.OriginalId;
            _file.CompanyId = file.CompanyId;
            _file.Extension = file.Extension;
            return this;
        }

        private void Reset()
        {
            _file = new ImageFile();
        }
    }

    public static class PathHelper
    {
        public static string CombineUrl(params string[] uriParts)
        {
            string uri = string.Empty;
            if (uriParts != null && uriParts.Any())
            {
                char[] trims = new char[] { '\\', '/' };
                uri = (uriParts[0] ?? string.Empty).TrimEnd(trims);

                for (int i = 1; i < uriParts.Length; i++)
                {
                    uri = string.Format("{0}/{1}", uri.TrimEnd(trims), (uriParts[i] ?? string.Empty).TrimStart(trims));
                }
            }

            uri = uri.Replace("\\", "/");
            return uri;
        }
    }
}
