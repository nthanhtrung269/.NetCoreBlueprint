using Newtonsoft.Json;

namespace Blueprint.AdapterDesignPattern.Services.PaymentProcessing.Adapters.FirstData
{
    /// <summary>
    /// Document: https://developer.cardconnect.com/cardconnect-api#capture-request
    /// </summary>
    public class FirstDataCaptureRequest
    {
        /// <summary>
        /// Merchant ID, required for all requests. Must match merchid of transaction to be captured.
        /// </summary>
        [JsonProperty(PropertyName = "merchid")]
        public string MerchantId { get; set; }

        /// <summary>
        /// CardConnect retrieval reference number from authorization response.
        /// </summary>
        [JsonProperty(PropertyName = "retref")]
        public string RetrievalReferenceNumber { get; set; }

        /// <summary>
        /// Capture amount in decimal or in currency minor units (for example, USD Pennies or MXN Centavos).
        /// </summary>
        [JsonProperty(PropertyName = "amount")]
        public string Amount { get; set; }
    }
}
