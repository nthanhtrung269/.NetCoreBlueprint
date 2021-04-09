namespace Blueprint.AdapterDesignPattern.Services.PaymentProcessing.Adapters
{
    public class AuthorizeRequest : BasePaymentRequest
    {
        public decimal Amount { get; set; }

        public string MerchantId { get; set; }

        public string UserId { get; set; }

        public long OrderId { get; set; }

        public long TransactionId { get; set; }

        public bool IsHold { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string OrdersNumber { get; set; }

        public string DescriptionPayment { get; set; }

        public string PaymentMethod { get; set; }

        public string TransactionIds { get; set; }
    }
}
