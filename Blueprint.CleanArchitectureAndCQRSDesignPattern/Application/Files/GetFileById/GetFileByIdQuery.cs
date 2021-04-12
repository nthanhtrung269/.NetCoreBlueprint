using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Queries;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.ReadModels;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.GetFileById
{
    public class GetFileByIdQuery : IQuery<BaseFileDto>
    {
        public int Id { get; }

        public GetFileByIdQuery(int id)
        {
            Id = id;
        }
    }
}
