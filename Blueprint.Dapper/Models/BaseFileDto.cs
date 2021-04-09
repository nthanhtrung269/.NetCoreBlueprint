namespace Blueprint.Dapper.Models
{
    public class BaseFileDto
    {
        public long Id { get; set; }

        public long? OriginalId { get; set; }

        public string OriginalFileName { get; set; }

        public int? Width { get; set; }

        public int? Height { get; set; }

        public string CompanyId { get; set; }

        public string FilePath { get; set; }

        public string Extension { get; set; }

        public string FileType { get; set; }

        public string CloudUrl { get; set; }
    }
}
