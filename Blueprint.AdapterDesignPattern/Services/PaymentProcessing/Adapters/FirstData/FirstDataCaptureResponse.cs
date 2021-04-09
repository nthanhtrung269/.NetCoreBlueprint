using Newtonsoft.Json;

namespace Blueprint.AdapterDesignPattern.Services.PaymentProcessing.Adapters.FirstData
{
    /// <summary>
    /// Document: https://developer.cardconnect.com/cardconnect-api#capture-response
    /// </summary>
    public class FirstDataCaptureResponse
    {
        /// <summary>
        /// Copied from the capture request.
        /// </summary>
        [JsonProperty(PropertyName = "merchid")]
        public string MerchantId { get; set; }

        /// <summary>
        /// Masked account number.
        /// </summary>
        [JsonProperty(PropertyName = "account")]
        public string AccountNumber { get; set; }

        /// <summary>
        /// The amount included in the capture request.
        /// </summary>
        [JsonProperty(PropertyName = "amount")]
        public string AmountToCapture { get; set; }

        /// <summary>
        /// The retref included in the capture request.
        /// </summary>
        [JsonProperty(PropertyName = "retref")]
        public string RetrievalReferenceNumber { get; set; }

        /// <summary>
        /// Automatically created and assigned unless otherwise specified.
        /// </summary>
        [JsonProperty(PropertyName = "batchid")]
        public string BatchId { get; set; }

        /// <summary>
        /// The current settlement status. 
        /// The settlement status changes throughout the transaction lifecycle, from authorization to settlement. 
        /// The following values can be returned in the capture response:
        /// Note: See Settlement Status Response Values for a complete list of setlstat values.
        /// Authorized - The authorization was approved, but the transaction has not yet been captured. 
        /// Declined - The authorization was declined; therefore, the transaction can not be captured.
        /// Queued for Capture - The authorization was approved and captured but has not yet been sent for settlement.
        /// Voided - The authorization was voided; therefore, the transaction cannot be captured.
        /// Zero Amount - The authorization was a $0 auth for account validation, which cannot be captured.
        /// </summary>
        [JsonProperty(PropertyName = "setlstat")]
        public string SettlementStatus { get; set; }

        /// <summary>
        /// A token that replaces the card number in capture and settlement requests, if requested.
        /// </summary>
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }

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
        /// Authorization code from original authorization request.
        /// </summary>
        [JsonProperty(PropertyName = "authcode")]
        public string AuthorizationCode { get; set; }
    }
}
