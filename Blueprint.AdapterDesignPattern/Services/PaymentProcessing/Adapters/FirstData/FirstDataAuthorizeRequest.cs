using Newtonsoft.Json;

namespace Blueprint.AdapterDesignPattern.Services.PaymentProcessing.Adapters.FirstData
{
    /// <summary>
    /// Document: https://developer.cardconnect.com/cardconnect-api#authorization-request
    /// </summary>
    public class FirstDataAuthorizeRequest
    {
        /// <summary>
        /// CardConnect merchant ID, required for all requests.
        /// </summary>
        [JsonProperty(PropertyName = "merchid")]
        public string MerchantId { get; set; }

        /// <summary>
        /// Amount with decimal or without decimal in currency minor units (for example, USD Pennies or EUR Cents). 
        /// The value can be a positive or negative amount or 0, and is used to identify the type of authorization, as follows:
        /// Positive - Authorization request.
        /// Zero - Account Verification request.See AVS and CVV settings to enable.Account Verification is not supported for eCheck (ACH) authorizations.
        /// Negative - Refund without reference (Forced Credit). Merchants must be configured to process forced credit transactions.To refund an existing authorization, use Refund.
        /// </summary>
        [JsonProperty(PropertyName = "amount")]
        public string Amount { get; set; }

        /// <summary>
        /// Card Expiration in either MMYY or YYYYMMDD format. Not required for eCheck (ACH) requests.
        /// </summary>
        [JsonProperty(PropertyName = "expiry")]
        public string CardExpiration { get; set; }

        /// <summary>
        /// Can be:
        /// CardSecure Token - Retrieved from the Bolt API, CardSecure API, or the Hosted iFrame Tokenizer
        /// Clear text card number
        /// Note: Only PCI Level 1 and Level 2 compliant merchants should handle clear text card numbers in authorization requests.
        /// It is strongly recommended that you tokenize the clear text card data before passing it in an authorization request.
        /// Bank Account Number - Account(s) must be entitled with electronic check capability. 
        /// When using this field, the bankaba field is also required.
        /// See Processing ACH Payments in the CardPointe Gateway Developer Guides for more information on ACH transactions.
        /// Note: To use a CardConnect profile, omit the account property and supply the profile ID in the profile field instead. 
        /// See Profiles for more information.
        /// </summary>
        [JsonProperty(PropertyName = "account")]
        public string Account { get; set; }

        /// <summary>
        /// Currency of the authorization (for example, USD for US dollars or CAD for Canadian Dollars).
        /// Note: If specified in the auth request, the currency value must match the currency that the MID is configured for. 
        /// Specifying the incorrect currency will result in a "Wrong currency for merch" response.
        /// </summary>
        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }

        /// <summary>
        /// Account holder's name, optional for credit cards and electronic checks (ACH).
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string AccountHolderName { get; set; }

        /// <summary>
        /// Optional, specify Y to capture the transaction for settlement if approved.
        /// </summary>
        [JsonProperty(PropertyName = "capture")]
        public string Capture { get; set; }

        /// <summary>
        /// Optional, to return receipt data fields in the authorization response. 
        /// Specify Y to return additional merchant and authorization data to print on a receipt.
        /// Defaults to N if not provided.
        /// </summary>
        [JsonProperty(PropertyName = "receipt")]
        public string Receipt { get; set; }

        /// <summary>
        /// Source system order number.
        /// Note: If you include an order ID it must meet the following requirements:
        /// The order ID must be a unique value.
        /// Using duplicate order IDs can lead to the wrong transaction being voided in the event of a timeout.
        /// The order ID must not include any portion of a payment account number(PAN), 
        /// and no portion of the order ID should be mistaken for a PAN.
        /// If the order ID passes the Luhn check performed by the CardPointe Gateway, 
        /// the value will be masked in the database, 
        /// and attempts to use the order ID in an inquire, void, or refund request will fail.
        /// </summary>
        [JsonProperty(PropertyName = "orderId")]
        public string OrderId { get; set; }
    }
}
