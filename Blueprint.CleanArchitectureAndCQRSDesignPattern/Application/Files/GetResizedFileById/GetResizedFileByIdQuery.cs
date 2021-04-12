using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Queries;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.GetResizedFileById
{
    public class GetResizedFileByIdQuery : IQuery<string>
    {
        public long ImageId { set; get; }
        public string Query { set; get; }

        public GetResizedFileByIdQuery(long id, string query)
        {
            ImageId = id;
            Query = query;
        }
    }
}
