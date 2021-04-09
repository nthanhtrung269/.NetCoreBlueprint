namespace Blueprint.AdapterDesignPattern.Services.PaymentProcessing.Adapters
{
    public class CreateNewCardRequest : BasePaymentRequest
    {
        public string CustomerId { get; set; }

        public string UserId { get; set; }
    }
}
