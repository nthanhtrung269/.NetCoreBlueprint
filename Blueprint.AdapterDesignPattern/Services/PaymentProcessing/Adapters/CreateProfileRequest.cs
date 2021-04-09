namespace Blueprint.AdapterDesignPattern.Services.PaymentProcessing.Adapters
{
    public class CreateProfileRequest : BasePaymentRequest
    {
        public string Email { get; set; }

        public string CustomerId { get; set; }
    }
}
