using Newtonsoft.Json;

namespace Blueprint.AdapterDesignPattern.Services.PaymentProcessing.Adapters.FirstData
{
    /// <summary>
    /// Document: https://developer.cardconnect.com/cardconnect-api#void-request
    /// </summary>
    public class FirstDataVoidRequest
    {
        /// <summary>
        /// Merchant ID, required for all requests.
        /// Must match merchid of the transaction to be voided.
        /// </summary>
        [JsonProperty(PropertyName = "merchid")]
        public string MerchantId { get; set; }

        /// <summary>
        /// CardConnect retrieval reference number from authorization response.
        /// </summary>
        [JsonProperty(PropertyName = "retref")]
        public string RetrievalReferenceNumber { get; set; }

        /// <summary>
        /// Optional, if omitted or equal to $0, the full amount is voided.
        /// If no capture has taken place(setlstat:Authorized), 
        /// you can specify a partial amount to void to reduce the amount of the authorization.
        /// </summary>
        [JsonProperty(PropertyName = "amount")]
        public string Amount { get; set; }
    }
}
