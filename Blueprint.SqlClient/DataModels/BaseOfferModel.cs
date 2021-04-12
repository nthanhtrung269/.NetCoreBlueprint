using System;
using System.Collections.Generic;

namespace Blueprint.SqlClient.DataModels
{
    public class BaseOfferModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string FullDescription { get; set; }

        public string OfferCode { get; set; }

        public int? OfferSegment { get; set; }

        public string OfferSegmentName { get; set; }

        public int? AdCode { get; set; }

        public string AdCodeDescription { get; set; }

        public string ImageUrl { get; set; }

        public string OfferDiscountPricing { get; set; }

        public string OfferSize { get; set; }

        public string OfferDisclaimer { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public List<string> ProductCodes { get; set; }

        public bool UpdateProductCodes { get; set; }

        public List<string> PrimaryUpcs { get; set; }

        public List<string> StoreCodes { get; set; }

        public bool UpdateStoreCodes { get; set; }

        public bool IsActive { get; set; }

        public int? MemberAccount { get; set; }

        public int? CustomerOfferFileId { get; set; }

        public bool? CustomerOfferFileStatus { get; set; }
    }
}
