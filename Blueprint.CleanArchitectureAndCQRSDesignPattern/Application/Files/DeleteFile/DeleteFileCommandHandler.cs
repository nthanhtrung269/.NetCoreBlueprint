using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Commands;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Database;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Interfaces;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Helpers;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Models;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.DeleteFile
{
    public class DeleteFileCommandHandler : ICommandHandler<DeleteFileCommand>
    {
        private readonly IFileRepository _fileRepository;
        private readonly IFileSystemService _fileSystemService;

        public DeleteFileCommandHandler(IFileRepository fileRepository,
            IFileSystemService fileSystemService)
        {
            _fileRepository = fileRepository;
            _fileSystemService = fileSystemService;
        }

        public async Task<Unit> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
        {
            Guard.AgainstNull(nameof(DeleteFileCommand), request);
            BlueprintFile blueprintFile = await _fileRepository.GetByIdAsync(request.FileId);
            Guard.AgainstNull(nameof(blueprintFile), blueprintFile);

            // Deletes file from forder
            _fileSystemService.Delete(blueprintFile.FilePath);

            await _fileRepository.DeleteAsync(blueprintFile);
            await _fileRepository.CommitAsync();

            return Unit.Value;
        }
    }
}
