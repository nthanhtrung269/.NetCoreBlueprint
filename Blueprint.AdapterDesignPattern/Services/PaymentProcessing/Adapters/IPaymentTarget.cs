namespace Blueprint.AdapterDesignPattern.Services.PaymentProcessing.Adapters
{
    public interface IPaymentTarget
    {
        GetProfileResponse GetProfile(GetProfileRequest getProfileRequest);

        CreateProfileResponse CreateProfile(CreateProfileRequest createProfileRequest);

        CreateNewCardResponse CreateNewCard(CreateNewCardRequest createNewCardRequest);

        AuthorizeResponse Authorize(AuthorizeRequest authorizeRequest);

        CaptureResponse Capture(CaptureRequest captureRequest);

        CancelResponse Cancel(CancelRequest cancelRequest);

        RefundResponse Refund(RefundRequest refundRequest);
    }
}
