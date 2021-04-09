using Newtonsoft.Json;

namespace Blueprint.AdapterDesignPattern.Services.PaymentProcessing.Adapters.FirstData
{
    /// <summary>
    /// Document: https://developer.cardconnect.com/cardconnect-api#authorization-response
    /// </summary>
    public class FirstDataAuthorizeResponse
    {
        /// <summary>
        /// Indicates the status of the authorization request. Can be one of the following values:
        /// A - Approved
        /// B - Retry
        /// C - Declined
        /// </summary>
        [JsonProperty(PropertyName = "respstat")]
        public string Status { get; set; }

        /// <summary>
        /// The unique retrieval reference number, used to identify and manage the transaction.
        /// </summary>
        [JsonProperty(PropertyName = "retref")]
        public string RetrievalReferenceNumber { get; set; }

        /// <summary>
        /// Either the masked payment card or eCheck (ACH) account 
        /// if "tokenize" : "y" in the request OR the token generated for the account if "tokenize" : "n" or was omitted in the request.
        /// </summary>
        [JsonProperty(PropertyName = "account")]
        public string AccountNumber { get; set; }

        /// <summary>
        /// The payment card expiration date, in MMYY format, if expiry was included in the request.
        /// </summary>
        [JsonProperty(PropertyName = "expiry")]
        public string ExpirationDate { get; set; }

        /// <summary>
        /// A token that replaces the card number in capture and settlement requests, if requested.
        /// </summary>
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }

        /// <summary>
        /// Authorized amount. Same as the request amount for most approvals.
        /// The amount remaining on the card for prepaid/gift cards if partial authorization is enabled.
        /// Not relevant for declines.
        /// </summary>
        [JsonProperty(PropertyName = "amount")]
        public string Amount { get; set; }

        /// <summary>
        /// Copied from the authorization request.
        /// </summary>
        [JsonProperty(PropertyName = "merchid")]
        public string MerchantId { get; set; }

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
        /// Alpha-numeric AVS (zip code) verification response code.
        /// Note: avsresp is typically only returned for approved authorizations, 
        /// however this field can be returned in the response for a declined authorization 
        /// if this setting is enabled for the merchant account.
        /// </summary>
        [JsonProperty(PropertyName = "avsresp")]
        public string AvsResponseCode { get; set; }

        /// <summary>
        /// Alpha-numeric CVV (card verification value) verification response code, if returned by the processor.
        /// One of the following values:
        /// M - Valid CVV Match.
        /// N - Invalid CVV.
        /// P - CVV Not Processed.
        /// S - Merchant indicated that the CVV is not present on the card.
        /// U - Card issuer is not certified and/or has not provided Visa encryption keys.
        /// X - No response. 
        /// Note: cvvresp is typically only returned for approved authorizations, 
        /// however this field can be returned in the response for a declined authorization 
        /// if this setting is enabled for the merchant account.
        /// </summary>
        [JsonProperty(PropertyName = "cvvresp")]
        public string CvvResponseCode { get; set; }

        /// <summary>
        /// Authorization Code from the Issuer.
        /// </summary>
        [JsonProperty(PropertyName = "authcode")]
        public string AuthorizationCode { get; set; }

        /// <summary>
        /// Y if a Corporate or Purchase Card.
        /// </summary>
        [JsonProperty(PropertyName = "commcard")]
        public string CommercialCardFlag { get; set; }

        /// <summary>
        /// An object that includes additional fields to be printed on a receipt.
        /// Refer to Receipt Data Fields below for a list of the possible fields returned.
        /// </summary>
        [JsonProperty(PropertyName = "receipt")]
        public string ReceiptData { get; set; }
    }
}
