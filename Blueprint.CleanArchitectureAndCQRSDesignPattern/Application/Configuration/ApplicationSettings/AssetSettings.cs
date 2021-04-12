using System.Collections.Generic;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.ApplicationSettings
{
    public class AssetSettings
    {
        public AssetSettings()
        {
            RatioRange = new List<string>();
            WidthRange = new List<int>();
            HeightRange = new List<int>();
            FileTypes = new List<string>();
        }

        public List<string> RatioRange { get; set; }

        public List<int> WidthRange { get; set; }

        public List<int> HeightRange { get; set; }

        public List<int> PreGeneratedWidthRange { get; set; }

        public List<int> PreGeneratedHeightRange { get; set; }

        public List<string> FileTypes { get; set; }

        public List<string> SupportedCategoryTypes { get; set; }

        public string RootDataFolder { get; set; }

        public string TempDataFolder { get; set; }

        public string CloudDataUrl { get; set; }

        public long FileSizeLimit { get; set; }

        public int FileProcessingHostedServiceTimeSpanInSeconds { get; set; }

        public int FileProcessingHostedServiceTotalFilesProcessedPerTimes { get; set; }
    }
}
