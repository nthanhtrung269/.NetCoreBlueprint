using System;
using System.IO;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.WriteModels
{
    public class FileDto
    {
        public long Id { get; set; }

        public long? OriginalId { get; set; }

        public string OriginalFileName { get; set; }

        public int? Width { get; set; }

        public int? Height { get; set; }

        public string CompanyId { get; set; }

        public byte[] FileData { get; set; }

        public Stream StreamData { set; get; }

        public string ThumbnailPath { get; set; }

        public string FilePath { get; set; }

        public string Extension { get; set; }

        public string FileType { get; set; }

        public string Source { get; set; }

        public string CloudUrl { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string FileName { set; get; }
    }
}