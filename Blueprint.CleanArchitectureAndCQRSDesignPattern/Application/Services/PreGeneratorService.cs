using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.ApplicationSettings;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Database;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Interfaces;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Enums;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Helpers;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Services
{
    public class PreGeneratorService : IPreGeneratorService
    {
        private readonly IFileRepository _fileRepository;
        private readonly ILogger<PreGeneratorService> _logger;
        private readonly IImageService _imageService;
        private readonly AssetSettings _configuration;
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        private const int PageSize = 50;

        public PreGeneratorService(IOptions<AssetSettings> config,
            IFileRepository fileRepository,
            ILogger<PreGeneratorService> logger,
            IImageService imageService,
            ISqlConnectionFactory sqlConnectionFactory)
        {
            _configuration = config.Value;
            _fileRepository = fileRepository;
            _logger = logger;
            _imageService = imageService;
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        /// <summary>
        /// Processes for PreGenerator.
        /// </summary>
        /// <returns>Task.</returns>
        public async Task Process()
        {
            _logger.LogInformation($"Call to {nameof(Process)}");

            List<string> supportFiles = new List<string>() { "jpg", "jpeg", "tif", "gif", "jfif", "webp", "png" };
            List<string> supportFileTypes = new List<string>() { "Product", "Service", "Collection" };
            int minWidth = GetMinWidth();
            int minHeight = GetMinHeight();

            // Clean up existing resized images data
            List<BlueprintFile> cleanUpFiles = await _fileRepository.FindByAsync(f => f.OriginalId != null);

            if (cleanUpFiles != null)
            {
                _logger.LogInformation($"Total items for cleaning up: {cleanUpFiles.Count}");

                for (int i = 0; i * PageSize <= cleanUpFiles.Count; i++)
                {
                    _logger.LogInformation($"Start ProcessCleanUpDataWithPageSize with PageIndex: {i}, PageSize: {PageSize}");
                    await ProcessCleanUpDataWithPageSize(cleanUpFiles.Skip(i * PageSize).Take(PageSize).ToList());
                }
            }

            // Start re-generating resized images data
            var connection = _sqlConnectionFactory.GetNewConnection();
            const string query = @"SELECT * FROM BlueprintFiles f WHERE f.OriginalId IS NULL
                                    AND f.Width IS NOT NULL
                                    AND f.Height IS NOT NULL
                                    AND (f.Width >= @MinWidth
                                            OR f.Height >= @MinHeight)
                                    AND (f.Extension in (SELECT * FROM STRING_SPLIT(@SupportFiles, ',')))
                                    AND (f.FileType in (SELECT * FROM STRING_SPLIT(@SupportFileTypes, ',')))";

            var rsFiles = (await connection.QueryAsync<BlueprintFile>(query, new
            {
                MinWidth = minWidth,
                MinHeight = minHeight,
                SupportFiles = string.Join(',',supportFiles),
                SupportFileTypes = string.Join(',', supportFileTypes)
            })).ToList();

            connection.Dispose();

            if (rsFiles != null)
            {
                _logger.LogInformation($"Total items for re-generating: {rsFiles.Count}");

                for (int i = 0; i * PageSize <= rsFiles.Count; i++)
                {
                    _logger.LogInformation($"Start ProcessWithPageSize with PageIndex: {i}, PageSize: {PageSize}");
                    await ProcessWithPageSize(rsFiles.Skip(i * PageSize).Take(PageSize).ToList());
                }
            }

            _logger.LogInformation($"End the call {nameof(Process)}");
        }

        /// <summary>
        /// Pregenerates resized images.
        /// </summary>
        /// <param name="rsFile">The rsFile.</param>
        /// <returns>Task.</returns>
        public async Task PreGenerateResizedImages(BlueprintFile rsFile)
        {
            await PreGenerateForOriginalFile(rsFile);
        }

        private async Task ProcessCleanUpDataWithPageSize(List<BlueprintFile> rsFiles)
        {
            _logger.LogInformation($"Call to {nameof(ProcessCleanUpDataWithPageSize)}");
            var tasks = new List<Task>();

            foreach (BlueprintFile rsFile in rsFiles)
            {
                tasks.Add(CleanUpDataForOriginalFile(rsFile));
            }

            var completionTask = Task.WhenAll(tasks);

            try
            {
                await completionTask;
            }
            catch
            {
                _logger.LogError(string.Join(", ", completionTask.Exception.Flatten().InnerExceptions.Select(e => e.Message)));
            }

            var connection = _sqlConnectionFactory.GetNewConnection();
            const string removeQuery = @"DELETE BlueprintFiles WHERE OriginalId is not null";
            await connection.ExecuteAsync(removeQuery);
            connection.Dispose();
        }

        private async Task ProcessWithPageSize(List<BlueprintFile> rsFiles)
        {
            _logger.LogInformation($"Call to {nameof(ProcessWithPageSize)}");
            var tasks = new List<Task>();

            foreach (BlueprintFile rsFile in rsFiles)
            {
                tasks.Add(PreGenerateForOriginalFile(rsFile));
            }

            var completionTask = Task.WhenAll(tasks);

            try
            {
                await completionTask;
            }
            catch
            {
                _logger.LogError(string.Join(", ", completionTask.Exception.Flatten().InnerExceptions.Select(e => e.Message)));
            }
        }

        private Task CleanUpDataForOriginalFile(BlueprintFile rsFile)
        {
            _logger.LogInformation($"Start {nameof(CleanUpDataForOriginalFile)} with Id: {rsFile.Id}");

            if (File.Exists(rsFile.FilePath))
            {
                File.Delete(rsFile.FilePath);
            }

            return Task.CompletedTask;
        }

        private async Task PreGenerateForOriginalFile(BlueprintFile rsFile)
        {
            _logger.LogInformation($"Start {nameof(PreGenerateForOriginalFile)} with Id: {rsFile.Id}");

            await DeleteResizedFilesAsync(rsFile);
            FileStream fileStream = _imageService.ReadImage(rsFile.FilePath);

            if (fileStream != null)
            {
                using (fileStream)
                {
                    foreach (int newWidth in _configuration.PreGeneratedWidthRange)
                    {
                        if (newWidth >= rsFile.Width)
                        {
                            continue;
                        }

                        await ResizeImageAsync(rsFile, fileStream, newWidth, 0);
                        fileStream.Position = 0;
                    }

                    foreach (int newHeight in _configuration.PreGeneratedHeightRange)
                    {
                        if (newHeight >= rsFile.Height)
                        {
                            continue;
                        }

                        await ResizeImageAsync(rsFile, fileStream, 0, newHeight);
                        fileStream.Position = 0;
                    }
                }
            }

            await UpdateFileAsync(rsFile);
        }

        private int GetMinWidth()
        {
            int? minValue = _configuration.PreGeneratedWidthRange.OrderBy(s => s).FirstOrDefault();
            return minValue ?? 0;
        }

        private int GetMinHeight()
        {
            int? minValue = _configuration.PreGeneratedHeightRange.OrderBy(s => s).FirstOrDefault();
            return minValue ?? 0;
        }

        private async Task DeleteResizedFilesAsync(BlueprintFile rsFile)
        {
            _logger.LogInformation($"Start DeleteResizedFilesAsync");

            var connection = _sqlConnectionFactory.GetNewConnection();
            const string getQuery = @"SELECT Id, FilePath
                               FROM BlueprintFiles
                               WHERE OriginalId = @Id";
            const string removeQuery = @"DELETE BlueprintFiles WHERE OriginalId = @Id";

            var query = await connection.QueryAsync<DeletedRsFile>(getQuery, new { rsFile.Id });
            var rsFiles = query.ToList();
            rsFiles.ForEach(f =>
            {
                if (File.Exists(f.FilePath))
                {
                    File.Delete(f.FilePath);
                }
            });
            await connection.ExecuteAsync(removeQuery, new { rsFile.Id });

            connection.Dispose();
        }

        private async Task UpdateFileAsync(BlueprintFile rsFile)
        {
            _logger.LogInformation($"Start UpdateFileAsync");

            var connection = _sqlConnectionFactory.GetNewConnection();
            const string updateQuery = @"UPDATE [dbo].[BlueprintFiles] SET BackgroudProcessingStatus = @BackgroudProcessingStatus, ModifiedBy=@ModifiedBy, ModifiedDate=@ModifiedDate WHERE Id = @Id";
            await connection.ExecuteAsync(updateQuery, new { BackgroudProcessingStatus = BackgroudProcessingStatus.Processed, ModifiedBy = nameof(System), ModifiedDate = DateTime.UtcNow, rsFile.Id });
            connection.Dispose();
        }

        private async Task ResizeImageAsync(BlueprintFile rsFile, FileStream fileStream, int newWidth, int newHeight)
        {
            _logger.LogInformation($"Start ResizeImageAsync: {rsFile.Id} - {rsFile.OriginalFileName} with width/height: {rsFile.Width}/{rsFile.Height} and newWidth/newHeight: {newWidth}/{newHeight}");

            var oldFileName = Path.GetFileName(rsFile.FilePath);
            string fileExtension = Path.GetExtension(rsFile.FilePath);
            var imagePath = Path.GetDirectoryName(rsFile.FilePath);

            var newFileName = string.Concat(Path.GetFileNameWithoutExtension(oldFileName),
                    newWidth == 0 ? "" : $"_w{newWidth}",
                    newHeight == 0 ? "" : $"_h{newHeight}",
                    fileExtension);

            string cloudPath = PathHelper.CombineUrl(imagePath.Replace(_configuration.RootDataFolder, _configuration.CloudDataUrl), string.Empty);

            if (File.Exists(Path.Combine(imagePath, newFileName)))
            {
                File.Delete(Path.Combine(imagePath, newFileName));
            }

            using (FileStream outputStream = new FileStream(Path.Combine(imagePath, newFileName), FileMode.CreateNew))
            {
                (int width, int height) dimentions = await _imageService.ResizeImage(fileStream, (newWidth, newHeight), fileExtension, outputStream);

                var filebuilder = new FileBuilder();
                var newFile = filebuilder.BuildCloudUrl(cloudPath, newFileName)
                    .BuildFilePath(imagePath, newFileName)
                    .BuildFileInfo(new BlueprintFile
                    {
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = nameof(System),
                        FileType = rsFile.FileType,
                        Height = dimentions.height,
                        Width = dimentions.width,
                        OriginalFileName = newFileName,
                        Source = rsFile.Source,
                        OriginalId = rsFile.Id,
                        Extension = rsFile.Extension,
                        CompanyId = rsFile.CompanyId,
                        FileName = null
                    })
                    .Build();

                await InsertRsFile(newFile);
            }
        }

        private async Task InsertRsFile(BlueprintFile rsFile)
        {
            _logger.LogInformation($"Saving rsFile to DB for Img: {rsFile.FilePath}");
            var connection = _sqlConnectionFactory.GetNewConnection();
            const string insertQuery = @"INSERT INTO [dbo].[BlueprintFiles]
                        ([OriginalId], [OriginalFileName], [Width], [Height], [CompanyId], [FilePath], [ThumbnailPath], [Extension], [FileType], [Source], [CloudUrl], [FileData], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) 
                        VALUES (@OriginalId, @OriginalFileName, @Width, @Height, @CompanyId, @FilePath, @ThumbnailPath, @Extension, @FileType, @Source, @CloudUrl, @FileData, @CreatedBy, @CreatedDate, @ModifiedBy, @ModifiedDate)";
            await connection.ExecuteAsync(insertQuery, rsFile);
            connection.Dispose();
        }
    }
}
