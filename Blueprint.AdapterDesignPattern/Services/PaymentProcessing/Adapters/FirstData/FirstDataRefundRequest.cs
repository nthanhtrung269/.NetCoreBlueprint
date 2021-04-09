using Newtonsoft.Json;

namespace Blueprint.AdapterDesignPattern.Services.PaymentProcessing.Adapters.FirstData
{
    /// <summary>
    /// Document: https://developer.cardconnect.com/cardconnect-api#refund-request
    /// </summary>
    public class FirstDataRefundRequest
    {
        /// <summary>
        /// Merchant ID, required for all requests
        /// </summary>
        [JsonProperty(PropertyName = "merchid")]
        public string MerchantId { get; set; }

        /// <summary>
        /// Retrieval Reference Number from original authorization.
        /// </summary>
        [JsonProperty(PropertyName = "retref")]
        public string RetrievalReferenceNumber { get; set; }

        /// <summary>
        /// Positive amount with decimal or amount without decimal in currency minor units 
        /// (for example, USD Pennies or MXN Centavos) for partial refunds.
        /// If no value is provided, the full amount of the transaction is refunded.
        /// </summary>
        [JsonProperty(PropertyName = "amount")]
        public string Amount { get; set; }
    }
}
