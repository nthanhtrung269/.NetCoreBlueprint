using System;

namespace Blueprint.AdapterDesignPattern.Models
{
    public class MerchantAccount
    {
        public MerchantAccountType MerchantAccountType { get; set; }

        public void SetMerchantAccountType(string provider)
        {
            if (provider.Equals(PaymentProviderConstants.Stripe, StringComparison.OrdinalIgnoreCase))
            {
                MerchantAccountType = MerchantAccountType.Stripe;
            }
            else if (provider.Equals(PaymentProviderConstants.FirstData, StringComparison.OrdinalIgnoreCase))
            {
                MerchantAccountType = MerchantAccountType.FirstData;
            }
        }
    }
}
