using Newtonsoft.Json;

namespace Blueprint.AdapterDesignPattern.Services.PaymentProcessing.Adapters.FirstData
{
    /// <summary>
    /// Document: https://developer.cardconnect.com/cardconnect-api#refund-response
    /// </summary>
    public class FirstDataRefundResponse
    {
        /// <summary>
        /// Copied from refund request.
        /// </summary>
        [JsonProperty(PropertyName = "merchid")]
        public string MerchantId { get; set; }

        /// <summary>
        /// Copied from refund request, contains the refund amount.
        /// </summary>
        [JsonProperty(PropertyName = "amount")]
        public string Amount { get; set; }

        /// <summary>
        /// New retref of refund transaction.
        /// </summary>
        [JsonProperty(PropertyName = "retref")]
        public string RetrievalReferenceNumber { get; set; }

        /// <summary>
        /// Alpha-numeric response code that represents the description of the response.
        /// </summary>
        [JsonProperty(PropertyName = "respcode")]
        public string ResponseCode { get; set; }

        /// <summary>
        /// Text description of response.
        /// </summary>
        [JsonProperty(PropertyName = "resptext")]
        public string ResponseText { get; set; }

        /// <summary>
        /// Abbreviation that represents the platform and the processor for the transaction.
        /// </summary>
        [JsonProperty(PropertyName = "respproc")]
        public string ResponseProcessor { get; set; }

        /// <summary>
        /// Indicates the status of the request. Can be one of the following values:
        /// A - Approved
        /// B - Retry
        /// C - Declined
        /// </summary>
        [JsonProperty(PropertyName = "respstat")]
        public string ResponseStatus { get; set; }
    }
}
