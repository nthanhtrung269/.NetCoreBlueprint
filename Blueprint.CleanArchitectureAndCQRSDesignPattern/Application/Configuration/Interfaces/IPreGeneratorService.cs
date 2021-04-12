using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Models;
using System.Threading.Tasks;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Interfaces
{
    public interface IPreGeneratorService
    {
        Task Process();

        Task PreGenerateResizedImages(BlueprintFile blueprintFile);
    }
}
