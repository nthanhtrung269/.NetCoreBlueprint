using System;

namespace Blueprint.AdapterDesignPattern.Services.PaymentProcessing.Adapters
{
    public class RefundResponse : BasePaymentResponse
    {
        public string Status { get; set; }

        public DateTime PaymentDate { get; set; }
    }
}
