using Microsoft.Extensions.Logging;
using Blueprint.AdapterDesignPattern.Models;
using Blueprint.AdapterDesignPattern.Services.PaymentProcessing;
using Blueprint.AdapterDesignPattern.Services.PaymentProcessing.Adapters;

namespace Blueprint.AdapterDesignPattern.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ILogger<PaymentService> _logger;
        private readonly IPaymentAdapterFactory _paymentAdapterFactory;

        public PaymentService(ILogger<PaymentService> logger,
            IPaymentAdapterFactory paymentAdapterFactory)
        {
            _logger = logger;
            _paymentAdapterFactory = paymentAdapterFactory;
        }

        private IPaymentTarget BuildPaymentAdapter(UserPaymentDto userPayment)
        {
            var mechantAccount = new MerchantAccount();
            mechantAccount.SetMerchantAccountType(userPayment.Provider);
            IPaymentTarget paymentTarget = _paymentAdapterFactory.BuildPaymentAdapter(mechantAccount);
            return paymentTarget;
        }
    }
}
