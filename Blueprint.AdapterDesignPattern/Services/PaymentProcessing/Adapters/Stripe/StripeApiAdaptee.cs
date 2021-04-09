using Blueprint.AdapterDesignPattern.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using System;
using System.Collections.Generic;

namespace Blueprint.AdapterDesignPattern.Services.PaymentProcessing.Adapters.Stripe
{
    public class StripeApiAdaptee : IStripeApiAdaptee
    {
        private readonly MerchantAccountSettings _appSettings;
        private readonly ILogger<StripeApiAdaptee> _logger;

        public StripeApiAdaptee(IOptions<MerchantAccountSettings> options,
            ILogger<StripeApiAdaptee> logger)
        {
            _appSettings = options.Value;
            _logger = logger;
        }

        public Customer CreateProfile(string email, string customerId)
        {
            var customerService = new CustomerService();
            Customer customer = null;

            if (!string.IsNullOrEmpty(customerId))
            {
                customer = customerService.Get(customerId);
            }

            if (customer == null)
            {
                var customerOptions = new CustomerCreateOptions
                {
                    Email = email
                };
                customer = customerService.Create(customerOptions);
            }

            return customer;
        }

        public SetupIntent CreateNewCard(string customerId, string userId)
        {
            var setupIntentService = new SetupIntentService();
            var intentOptions = new SetupIntentCreateOptions
            {
                Customer = customerId
            };

            return setupIntentService.Create(intentOptions);
        }

        public SetupIntent GetProfile(string cardToken)
        {
            return new SetupIntentService().Get(cardToken);
        }

        public PaymentIntent Authorize(decimal payAmount,
            bool isHold,
            string email,
            string ordersNumber,
            string firstName,
            string lastName,
            string descriptionPayment,
            string paymentMethod,
            string externalId,
            string transactionIds,
            string userId,
            long orderId,
            long transactionId,
            Guid correlationId)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(payAmount * 100),
                Currency = "USD",
                CaptureMethod = isHold ? "manual" : "automatic",
                Confirm = true,
                ReceiptEmail = email,
                Description = $"Checkout order {ordersNumber}- Customer {firstName} {lastName} - {descriptionPayment}",
                PaymentMethod = paymentMethod,
                Customer = externalId,
                Metadata = new Dictionary<string, string>
                    {
                        {
                            "TransactionIds",
                            transactionIds
                        }
                    }
            };

            var service = new PaymentIntentService();
            PaymentIntent paymentIntent = service.Create(options);

            return paymentIntent;
        }

        public PaymentIntent Capture(decimal? payAmount,
            string paymentId,
            string userId,
            long orderId,
            long transactionId,
            Guid correlationId)
        {
            var options = new PaymentIntentCaptureOptions
            {
                AmountToCapture = (long)(payAmount * 100)
            };
            var service = new PaymentIntentService();

            var intent = service.Capture(paymentId, options);
            return intent;
        }

        public Refund Refund(decimal? payAmount,
            string paymentId,
            string userId,
            long orderId,
            long transactionId,
            Guid correlationId)
        {
            var refunds = new RefundService();
            var refundOptions = new RefundCreateOptions
            {
                PaymentIntent = paymentId,
                Amount = (long)(payAmount * 100)
            };

            var refund = refunds.Create(refundOptions);

            return refund;
        }

        public PaymentIntent Cancel(string paymentId,
            string userId,
            long orderId,
            long transactionId,
            Guid correlationId)
        {
            var service = new PaymentIntentService();
            var options = new PaymentIntentCancelOptions { };

            var intent = service.Cancel(paymentId, options);
            return intent;
        }
    }
}
