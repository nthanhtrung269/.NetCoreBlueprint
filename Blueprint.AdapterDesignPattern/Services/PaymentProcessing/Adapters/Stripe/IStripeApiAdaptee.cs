using Stripe;
using System;

namespace Blueprint.AdapterDesignPattern.Services.PaymentProcessing.Adapters.Stripe
{
    public interface IStripeApiAdaptee
    {
        Customer CreateProfile(string email, string customerId);

        SetupIntent CreateNewCard(string customerId, string userId);

        SetupIntent GetProfile(string cardToken);

        PaymentIntent Authorize(decimal payAmount,
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
           Guid correlationId);

        PaymentIntent Capture(decimal? payAmount,
            string paymentId,
            string userId,
            long orderId,
            long transactionId,
            Guid correlationId);

        Refund Refund(decimal? payAmount,
           string paymentId,
           string userId,
           long orderId,
           long transactionId,
           Guid correlationId);

        public PaymentIntent Cancel(string paymentId,
            string userId,
            long orderId,
            long transactionId,
            Guid correlationId);
    }
}
