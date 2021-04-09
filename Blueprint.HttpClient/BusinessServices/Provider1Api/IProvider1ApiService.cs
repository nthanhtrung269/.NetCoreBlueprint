using System.Threading.Tasks;

namespace Blueprint.HttpClient1.BusinessServices.Provider1Api
{
    public interface IProvider1ApiService
    {
        Task<Provider1AuthorizationResponse> GetAuthorizationInfo();

        Task<ProductResponseModel> SearchProduct(ProductsByProgramRun productsByProgramRun, string token);
    }
}
