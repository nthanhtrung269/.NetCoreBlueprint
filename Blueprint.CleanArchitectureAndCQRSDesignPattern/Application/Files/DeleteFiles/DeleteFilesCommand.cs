using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Commands;
using System.Collections.Generic;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.DeleteFiles
{
    public class DeleteFilesCommand : CommandBase
    {
        public IList<long> Ids { get; }

        public DeleteFilesCommand(IList<long> ids)
        {
            Ids = ids;
        }
    }
}
