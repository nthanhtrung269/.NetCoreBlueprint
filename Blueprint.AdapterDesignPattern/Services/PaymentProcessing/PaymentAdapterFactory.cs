using Microsoft.Extensions.DependencyInjection;
using Blueprint.AdapterDesignPattern.Models;
using Blueprint.AdapterDesignPattern.Services.PaymentProcessing.Adapters;
using Blueprint.AdapterDesignPattern.Services.PaymentProcessing.Adapters.FirstData;
using Blueprint.AdapterDesignPattern.Services.PaymentProcessing.Adapters.Stripe;
using System;

namespace Blueprint.AdapterDesignPattern.Services.PaymentProcessing
{
    public class PaymentAdapterFactory : IPaymentAdapterFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public PaymentAdapterFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IPaymentTarget BuildPaymentAdapter(MerchantAccount mechantAccount)
        {
            if (mechantAccount.MerchantAccountType == MerchantAccountType.Stripe)
            {
                return (IPaymentTarget)_serviceProvider.GetService<IStripeApiAdapter>();
            }

            if (mechantAccount.MerchantAccountType == MerchantAccountType.FirstData)
            {
                return (IPaymentTarget)_serviceProvider.GetService<IFirstDataApiAdapter>();
            }

            throw new InvalidOperationException($"Invalid MerchantAccount: {mechantAccount.MerchantAccountType}");
        }
    }
}
