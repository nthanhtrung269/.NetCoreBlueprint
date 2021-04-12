using System;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Settings
{
    public class SettingDto
    {
        public int Id { get; set; }

        public string SettingName { get; set; }

        public string SettingValue { get; set; }

        public int? DisplayOrder { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public bool? IsDeleted { get; set; }
    }
}