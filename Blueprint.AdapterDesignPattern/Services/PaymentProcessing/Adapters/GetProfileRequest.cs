namespace Blueprint.AdapterDesignPattern.Services.PaymentProcessing.Adapters
{
    public class GetProfileRequest : BasePaymentRequest
    {
        public string ProfileId { get; set; }

        public string Token { get; set; }

        public string MerchantId { get; set; }
    }
}
