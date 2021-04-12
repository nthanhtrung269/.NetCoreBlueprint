using MediatR;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Queries
{
    public interface IQuery<out TResult> : IRequest<TResult>
    {

    }
}