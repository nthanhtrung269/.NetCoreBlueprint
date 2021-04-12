using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Queries;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.ReadModels;
using System.Collections.Generic;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.GetMultipleFilesByIds
{
    public class GetMultipleFilesByIdsQuery : IQuery<IList<BaseFileDto>>
    {
        public IList<long> Ids { get; }

        public GetMultipleFilesByIdsQuery(IList<long> ids)
        {
            Ids = ids;
        }
    }
}
