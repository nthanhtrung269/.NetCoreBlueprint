using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.SharedKernel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Models
{
    public partial class BlueprintFile : AuditableEntity<long>, IAggregateRoot, IHasDomainEvent
    {
        public long? OriginalId { get; set; }
        public string OriginalFileName { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public string CompanyId { get; set; }
        public string FilePath { get; set; }
        public string ThumbnailPath { get; set; }
        public string Extension { get; set; }
        public string FileType { get; set; }
        public string Source { get; set; }
        public string CloudUrl { get; set; }
        public byte[] FileData { get; set; }
        public int? BackgroudProcessingStatus { get; set; }
        public string FileName { set; get; }

        [NotMapped]
        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}
