using Blueprint.AdapterDesignPattern.Services.PaymentProcessing.Adapters;
using Blueprint.AdapterDesignPattern.Models;

namespace Blueprint.AdapterDesignPattern.Services.PaymentProcessing
{
    public interface IPaymentAdapterFactory
    {
        IPaymentTarget BuildPaymentAdapter(MerchantAccount mechantAccount);
    }
}
