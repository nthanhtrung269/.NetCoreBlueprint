using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blueprint.HttpClient1.BusinessServices.Provider2Api
{
    public interface IProvider2ApiService
    {
        Task<List<ProductResponseModel>> SearchProducts(List<ProductsByProgramRun> productsByProgramRuns);
    }
}
