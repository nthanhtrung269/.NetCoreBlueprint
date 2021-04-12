using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Commands;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Database;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Interfaces;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Helpers;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Models;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.DeleteFiles
{
    public class DeleteFilesCommandHandler : ICommandHandler<DeleteFilesCommand>
    {
        private readonly IFileRepository _fileRepository;
        private readonly IFileSystemService _fileSystemService;

        public DeleteFilesCommandHandler(IFileRepository fileRepository, IFileSystemService fileSystemService)
        {
            _fileRepository = fileRepository;
            _fileSystemService = fileSystemService;
        }

        public async Task<Unit> Handle(DeleteFilesCommand request, CancellationToken cancellationToken)
        {
            Guard.AgainstNull(nameof(BlueprintFile), request);
            Guard.AgainstNullOrNotAny(nameof(BlueprintFile), request.Ids);

            var blueprintFiles = await _fileRepository.FindByAsync(e => request.Ids.Contains(e.Id));
            var tasks = new List<Task>();

            // Deletes files in folder
            tasks.Add(Task.Run(() => { blueprintFiles.ForEach(f => _fileSystemService.Delete(f.FilePath)); }));

            // Deletes data files in database
            tasks.Add(_fileRepository.DeleteFiles(blueprintFiles));

            Task task = Task.WhenAll(tasks);
            await task;

            return Unit.Value;
        }
    }
}
