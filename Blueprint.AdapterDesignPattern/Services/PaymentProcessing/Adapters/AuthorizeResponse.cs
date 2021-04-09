using System;

namespace Blueprint.AdapterDesignPattern.Services.PaymentProcessing.Adapters
{
    public class AuthorizeResponse : BasePaymentResponse
    {
        public string Status { get; set; }

        public string PaymentId { get; set; }

        public DateTime? PaymentDate { get; set; }

        public decimal? PaymentFee { get; set; }

        public string PaymentCardId { get; set; }
    }
}
