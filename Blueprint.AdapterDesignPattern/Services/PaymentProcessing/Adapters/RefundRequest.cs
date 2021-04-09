namespace Blueprint.AdapterDesignPattern.Services.PaymentProcessing.Adapters
{
    public class RefundRequest : BasePaymentRequest
    {
        public string PaymentId { get; set; }

        public decimal? Amount { get; set; }

        public string UserId { get; set; }

        public long OrderId { get; set; }

        public long TransactionId { get; set; }
    }
}
