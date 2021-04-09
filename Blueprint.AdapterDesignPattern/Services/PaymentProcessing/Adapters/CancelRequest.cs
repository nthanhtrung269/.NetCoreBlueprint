namespace Blueprint.AdapterDesignPattern.Services.PaymentProcessing.Adapters
{
    public class CancelRequest : BasePaymentRequest
    {
        public string PaymentId { get; set; }

        public string UserId { get; set; }

        public long OrderId { get; set; }

        public long TransactionId { get; set; }
    }
}
