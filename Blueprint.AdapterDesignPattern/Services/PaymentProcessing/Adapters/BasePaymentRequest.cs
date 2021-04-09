using System;

namespace Blueprint.AdapterDesignPattern.Services.PaymentProcessing.Adapters
{
    public class BasePaymentRequest
    {
        /// <summary>
        /// Unique Identifier used by logging.
        /// </summary>
        public Guid CorrelationId { get; set; }
    }
}
