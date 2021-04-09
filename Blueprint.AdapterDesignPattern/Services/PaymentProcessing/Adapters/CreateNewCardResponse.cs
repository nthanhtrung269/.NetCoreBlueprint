namespace Blueprint.AdapterDesignPattern.Services.PaymentProcessing.Adapters
{
    public class CreateNewCardResponse : BasePaymentResponse
    {
        public string SetupIntentId { get; set; }

        public string ClientSecret { get; set; }
    }
}
