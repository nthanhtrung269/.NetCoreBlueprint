namespace Blueprint.HttpClient1
{
    public class ProductResponseModel
    {
        public string PromotionId { get; set; }

        public string OfferId { get; set; }

        public string ProductCode { get; set; }

        public string Name { get; set; }

        public string Provider2ImageUrl { get; set; }

        public string Provider1ImageUrl { get; set; }

        public ProductType ProductType { get; set; }

        public ProductsByProgramRun ProductsByProgramRun { get; set; }
    }
}
