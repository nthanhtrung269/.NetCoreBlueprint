using System.Threading.Tasks;

namespace Blueprint.AdapterDesignPattern.Services.PaymentProcessing.Adapters.FirstData
{
    public interface IFirstDataApiAdaptee
    {
        Task<FirstDataAuthorizeResponse> Authorize(FirstDataAuthorizeRequest request);

        Task<FirstDataCaptureResponse> Capture(FirstDataCaptureRequest request);

        Task<FirstDataVoidResponse> Void(FirstDataVoidRequest request);

        Task<FirstDataRefundResponse> Refund(FirstDataRefundRequest request);
    }
}
