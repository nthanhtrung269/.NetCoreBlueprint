namespace Blueprint.AdapterDesignPattern.Models
{
    public class MerchantAccountSettings
    {
        public MerchantAccountApi StripeApi { get; set; }

        public MerchantAccountApi FirstDataApi { get; set; }
    }

    public class MerchantAccountApi
    {
        public string ProviderName { get; set; }

        public string ApiUrl { get; set; }

        public int ApiTimeoutInSeconds { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Authorization { get; set; }

        public string MerchantId { get; set; }

        public string Currency { get; set; }
    }
}
