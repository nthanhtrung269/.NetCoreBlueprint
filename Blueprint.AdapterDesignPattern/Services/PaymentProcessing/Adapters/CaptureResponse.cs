using System;

namespace Blueprint.AdapterDesignPattern.Services.PaymentProcessing.Adapters
{
    public class CaptureResponse : BasePaymentResponse
    {
        public string Status { get; set; }

        public DateTime PaymentDate { get; set; }
    }
}
