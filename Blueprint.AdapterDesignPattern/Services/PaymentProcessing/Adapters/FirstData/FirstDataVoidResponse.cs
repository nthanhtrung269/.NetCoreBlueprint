using Newtonsoft.Json;

namespace Blueprint.AdapterDesignPattern.Services.PaymentProcessing.Adapters.FirstData
{
    /// <summary>
    /// Document: https://developer.cardconnect.com/cardconnect-api#void-response
    /// </summary>
    public class FirstDataVoidResponse
    {
        /// <summary>
        /// Retrieved from void request.
        /// </summary>
        [JsonProperty(PropertyName = "merchid")]
        public string MerchantId { get; set; }

        /// <summary>
        /// The order ID, if included in the original authorization request.
        /// Note:
        /// "orderId" : "", If no order ID was provided in the original auth request and "receipt" : "y".
        /// orderId is not returned in the response if no order ID was provided in the original auth request and "receipt" : "n".
        /// </summary>
        [JsonProperty(PropertyName = "orderId")]
        public string OrderId { get; set; }

        /// <summary>
        /// The final amount authorized after the void. 
        /// If the full amount was voided, the amount value in the response is 0.00.
        /// If a partial amount was voided, the amount value in the response is the difference 
        /// between the original authorization amount and the amount voided.
        /// </summary>
        [JsonProperty(PropertyName = "amount")]
        public string Amount { get; set; }

        /// <summary>
        /// Retrieved from void request.
        /// </summary>
        [JsonProperty(PropertyName = "retref")]
        public string RetrievalReferenceNumber { get; set; }

        /// <summary>
        /// Identifies if the void was successful. Can one of the following values: 
        /// REVERS - Successful
        /// Null - Unsuccessful.Refer to the respcode and resptext.
        /// </summary>
        [JsonProperty(PropertyName = "authcode")]
        public string AuthorizationCode { get; set; }

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
