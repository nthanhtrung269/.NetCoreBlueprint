using System.Threading.Tasks;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Interfaces
{
    public interface IDimensionsUpdaterService
    {
        Task Process();
    }
}
