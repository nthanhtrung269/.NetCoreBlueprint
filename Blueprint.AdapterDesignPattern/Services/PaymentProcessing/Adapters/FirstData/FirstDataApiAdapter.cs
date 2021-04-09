using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Blueprint.AdapterDesignPattern.Models;
using System;

namespace Blueprint.AdapterDesignPattern.Services.PaymentProcessing.Adapters.FirstData
{
    public class FirstDataApiAdapter : IPaymentTarget, IFirstDataApiAdapter
    {
        private readonly MerchantAccountSettings _appSettings;
        private readonly ILogger<FirstDataApiAdapter> _logger;

        public FirstDataApiAdapter(IOptions<MerchantAccountSettings> options,
            ILogger<FirstDataApiAdapter> logger)
        {
            _appSettings = options.Value;
            _logger = logger;
        }

        public GetProfileResponse GetProfile(GetProfileRequest getProfileRequest)
        {
            throw new NotImplementedException();
        }

        public CreateProfileResponse CreateProfile(CreateProfileRequest createProfileRequest)
        {
            throw new NotImplementedException();
        }

        public CreateNewCardResponse CreateNewCard(CreateNewCardRequest createNewCardRequest)
        {
            throw new NotImplementedException();
        }

        public AuthorizeResponse Authorize(AuthorizeRequest authorizeRequest)
        {
            throw new NotImplementedException();
        }

        public CaptureResponse Capture(CaptureRequest captureRequest)
        {
            throw new NotImplementedException();
        }

        public CancelResponse Cancel(CancelRequest cancelRequest)
        {
            throw new NotImplementedException();
        }

        public RefundResponse Refund(RefundRequest refundRequest)
        {
            throw new NotImplementedException();
        }
    }
}
