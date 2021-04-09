using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Blueprint.AdapterDesignPattern.Models;
using Stripe;

namespace Blueprint.AdapterDesignPattern.Services.PaymentProcessing.Adapters.Stripe
{
    public class StripeApiAdapter : IPaymentTarget, IStripeApiAdapter
    {
        private readonly MerchantAccountSettings _appSettings;
        private readonly ILogger<StripeApiAdapter> _logger;
        private readonly IStripeApiAdaptee _adaptee;

        public StripeApiAdapter(IOptions<MerchantAccountSettings> options,
            ILogger<StripeApiAdapter> logger,
            IStripeApiAdaptee adaptee)
        {
            _appSettings = options.Value;
            _logger = logger;
            _adaptee = adaptee;
        }

        public GetProfileResponse GetProfile(GetProfileRequest getProfileRequest)
        {
            SetupIntent setupIntent = _adaptee.GetProfile(getProfileRequest.Token);

            return new GetProfileResponse()
            {
                PaymentMethod = setupIntent.PaymentMethodId
            };
        }

        public CreateProfileResponse CreateProfile(CreateProfileRequest createProfileRequest)
        {
            Customer customer = _adaptee.CreateProfile(createProfileRequest.Email, createProfileRequest.CustomerId);

            return new CreateProfileResponse()
            {
                CustomerId = customer.Id
            };
        }

        public CreateNewCardResponse CreateNewCard(CreateNewCardRequest createNewCardRequest)
        {
            SetupIntent setupIntent = _adaptee.CreateNewCard(createNewCardRequest.CustomerId, createNewCardRequest.UserId);

            return new CreateNewCardResponse()
            {
                SetupIntentId = setupIntent.Id,
                ClientSecret = setupIntent.ClientSecret
            };
        }

        public AuthorizeResponse Authorize(AuthorizeRequest authorizeRequest)
        {
            PaymentIntent paymentIntent = _adaptee.Authorize(authorizeRequest.Amount,
                authorizeRequest.IsHold,
                authorizeRequest.Email,
                authorizeRequest.OrdersNumber,
                authorizeRequest.FirstName,
                authorizeRequest.LastName,
                authorizeRequest.DescriptionPayment,
                authorizeRequest.PaymentMethod,
                authorizeRequest.MerchantId,
                authorizeRequest.TransactionIds,
                authorizeRequest.UserId,
                authorizeRequest.OrderId,
                authorizeRequest.TransactionId,
                authorizeRequest.CorrelationId);

            return new AuthorizeResponse()
            {
                Status = paymentIntent.Status,
                PaymentId = paymentIntent.Id,
                PaymentDate = paymentIntent.Created,
                PaymentCardId = paymentIntent.PaymentMethodId,
                PaymentFee = paymentIntent.ApplicationFeeAmount
            };
        }

        public CaptureResponse Capture(CaptureRequest captureRequest)
        {
            PaymentIntent paymentIntent = _adaptee.Capture(captureRequest.Amount,
                captureRequest.PaymentId,
                captureRequest.UserId,
                captureRequest.OrderId,
                captureRequest.TransactionId,
                captureRequest.CorrelationId);

            return new CaptureResponse()
            {
                Status = paymentIntent.Status
            };
        }

        public CancelResponse Cancel(CancelRequest cancelRequest)
        {
            PaymentIntent paymentIntent = _adaptee.Cancel(cancelRequest.PaymentId,
                cancelRequest.UserId,
                cancelRequest.OrderId,
                cancelRequest.TransactionId,
                cancelRequest.CorrelationId);

            return new CancelResponse()
            {
                Status = paymentIntent.Status
            };
        }

        public RefundResponse Refund(RefundRequest refundRequest)
        {
            Refund refund = _adaptee.Refund(refundRequest.Amount,
                refundRequest.PaymentId,
                refundRequest.UserId,
                refundRequest.OrderId,
                refundRequest.TransactionId,
                refundRequest.CorrelationId);

            return new RefundResponse()
            {
                Status = refund.Status
            };
        }
    }
}
