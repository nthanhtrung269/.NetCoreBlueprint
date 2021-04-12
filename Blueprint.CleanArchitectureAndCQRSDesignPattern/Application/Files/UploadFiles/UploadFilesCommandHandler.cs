using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.ApplicationSettings;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Commands;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Database;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Interfaces;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.WriteModels;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Enums;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Helpers;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Models;
using MapsterMapper;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.UploadFiles
{
    public class UploadFilesCommandHandler : ICommandHandler<UploadFilesCommand, UploadResponse>
    {
        private readonly IFileRepository _fileRepository;
        private readonly AssetSettings _configuration;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;
        private readonly IFileSystemService _fileSystemService;

        public UploadFilesCommandHandler(IFileRepository fileRepository,
            IOptions<AssetSettings> config,
            IMapper mapper,
            IImageService imageService,
            IFileSystemService fileSystemService)
        {
            _fileRepository = fileRepository;
            _configuration = config.Value;
            _mapper = mapper;
            _imageService = imageService;
            _fileSystemService = fileSystemService;
        }

        public async Task<UploadResponse> Handle(UploadFilesCommand request, CancellationToken cancellationToken)
        {
            // Step 1: Gets parameters from request
            Dictionary<string, StringValues> parameters = request.UploadResponse.FormAccumulator.GetResults();

            // Step 2: Validates request
            if (request.UploadResponse.Files == null)
            {
                return new UploadResponse();
            }

            // Step 3: Creates upload folder if not existed
            CreateUploadFolderIfNotExisted(GetAssetType(parameters));

            // Step 4: Creates returned object
            var result = new UploadResponse
            {
                Messages = request.UploadResponse.Messages,
                Files = new List<FileDto>(),
                Status = true
            };

            // Step 5: Processing for all uploaded files
            foreach (var fileUploadResponse in request.UploadResponse.Files)
            {
                FileDto fileToAdd = await GetFileDto(parameters, fileUploadResponse);
                bool requestStatus = await ValidateAndAddFileAsync(result, fileToAdd);

                if (!requestStatus)
                {
                    request.UploadResponse.Status = requestStatus;
                }
            }

            // Step 6: Sets response status and returns object
            result.Status = request.UploadResponse.Status;
            return result;
        }

        private async Task<FileDto> GetFileDto(Dictionary<string, StringValues> parameters, FileDto fileUploadResponse)
        {
            // Step 1: Moves file to target folder
            string targetFile = MoveFileToTargetFolder(parameters, fileUploadResponse);

            // Step 2: Gets file dimensions
            (int height, int width) dimensions = await GetDimensions(fileUploadResponse);

            // Step 3: Returns file DTO
            return new FileDto()
            {
                CompanyId = parameters["companyId"],
                FilePath = targetFile,
                FileType = GetAssetType(parameters).ToString(),
                Extension = fileUploadResponse.Extension,
                OriginalFileName = fileUploadResponse.OriginalFileName,
                Width = dimensions.width,
                Height = dimensions.height,
                FileName = fileUploadResponse.FileName
            };
        }

        private string MoveFileToTargetFolder(Dictionary<string, StringValues> parameters, FileDto fileUploadResponse)
        {
            string targetFile = Path.Combine(_configuration.RootDataFolder,
                            GetAssetType(parameters).ToString(),
                            GetNewFileNameWithExtension(GetFileNameWithoutExtension(fileUploadResponse.FilePath), Path.GetExtension(fileUploadResponse.FilePath)));

            _fileSystemService.Move(fileUploadResponse.FilePath, targetFile);
            return targetFile;
        }

        private async Task<(int height, int width)> GetDimensions(FileDto fileUploadResponse)
        {
            (int height, int width) dimensions = (0, 0);

            if (_configuration.FileTypes.Contains(fileUploadResponse.Extension))
            {
                using (var stream = fileUploadResponse.StreamData)
                {
                    dimensions = await _imageService.GetImageDimentions(stream, Path.GetExtension(fileUploadResponse.FilePath), CancellationToken.None);
                }
            }

            return dimensions;
        }

        private static string GetNewFileNameWithExtension(string fileNameWithoutExtension, string fileExtension)
        {
            return fileNameWithoutExtension + fileExtension;
        }

        private async Task<bool> ValidateAndAddFileAsync(UploadResponse result, FileDto fileToAdd)
        {
            bool requestStatus = true;
            FileDto addResult = await AddFileAsync(fileToAdd);

            if (addResult == null)
            {
                var messageRo = new UploadResponseMessage()
                {
                    Text = $"The file {fileToAdd.OriginalFileName} added failed.",
                    Code = "AddedFailed"
                };
                result.Messages.Add(messageRo);
                requestStatus = false;
            }
            else
            {
                if (string.IsNullOrEmpty(addResult.CloudUrl))
                {
                    string fileName = addResult.FilePath.Replace(_configuration.RootDataFolder, string.Empty);
                    addResult.CloudUrl = PathHelper.CombineUrl(_configuration.CloudDataUrl, fileName);
                }
                result.Files.Add(addResult);
            }

            return requestStatus;
        }

        private static string GetFileNameWithoutExtension(string tempPath)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(tempPath);
            fileNameWithoutExtension = Regex.Replace(fileNameWithoutExtension, @"[^a-zA-Z0-9_]", "");
            return fileNameWithoutExtension;
        }

        private void CreateUploadFolderIfNotExisted(AssetType typeFile)
        {
            string targetFolder = Path.Combine(_configuration.RootDataFolder, typeFile.ToString());
            if (!_fileSystemService.Exists(targetFolder))
            {
                _fileSystemService.CreateDirectory(targetFolder);
            }
        }

        private static AssetType GetAssetType(Dictionary<string, StringValues> parameters)
        {
            string type = parameters["type"];
            AssetType typeFile;
            bool tryParse = Enum.TryParse(type, result: out typeFile);
            if (!tryParse)
            {
                typeFile = AssetType.Default;
            }

            return typeFile;
        }

        private async Task<FileDto> AddFileAsync(FileDto fileToAdd)
        {
            var result = _mapper.Map<FileDto, BlueprintFile>(fileToAdd);
            result.CreatedDate = DateTime.UtcNow;

            if (!_configuration.FileTypes.Contains(fileToAdd.Extension))
                result.BackgroudProcessingStatus = (int?)BackgroudProcessingStatus.Processed;

            await _fileRepository.AddAsync(result);
            await _fileRepository.CommitAsync();

            return _mapper.Map<BlueprintFile, FileDto>(result);
        }
    }
}
