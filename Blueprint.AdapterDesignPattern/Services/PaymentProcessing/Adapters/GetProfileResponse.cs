namespace Blueprint.AdapterDesignPattern.Services.PaymentProcessing.Adapters
{
    public class GetProfileResponse : BasePaymentResponse
    {
        public string PaymentMethod { get; set; }

        public string ProfileId { get; set; }

        public string Token { get; set; }

        public string MerchantId { get; set; }
    }
}
