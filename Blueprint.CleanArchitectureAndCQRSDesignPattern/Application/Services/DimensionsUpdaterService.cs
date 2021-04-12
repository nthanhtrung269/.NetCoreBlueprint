using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Database;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.Interfaces;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Models;
using Dapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Services
{
    public class DimensionsUpdaterService : IDimensionsUpdaterService
    {
        private readonly IFileRepository _fileRepository;
        private readonly ILogger<DimensionsUpdaterService> _logger;
        private readonly IImageService _imageService;
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        private const int PageSize = 50;

        public DimensionsUpdaterService(IFileRepository fileRepository,
            ILogger<DimensionsUpdaterService> logger,
            IImageService imageService,
            ISqlConnectionFactory sqlConnectionFactory)
        {
            _fileRepository = fileRepository;
            _logger = logger;
            _imageService = imageService;
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task Process()
        {
            _logger.LogInformation($"Call to {nameof(Process)}");
            var timer = Stopwatch.StartNew();

            List<string> supportFiles = new List<string>() { "jpg", "jpeg", "tif", "gif", "jfif", "webp", "png" };
            List<BlueprintFile> blueprintFiles = await _fileRepository.FindByAsync(f => supportFiles.Contains(f.Extension) && (f.Width == null || f.Height == null));

            for (int i = 0; i * PageSize <= blueprintFiles.Count; i++)
            {
                _logger.LogInformation($"Start ProcessWithPageSize with PageIndex: {i}, PageSize: {PageSize}");
                await ProcessWithPageSize(blueprintFiles.Skip(i * PageSize).Take(PageSize).ToList());
            }

            timer.Stop();
            _logger.LogInformation($"End the call {nameof(Process)} after {timer.ElapsedMilliseconds}ms");
        }

        private async Task ProcessWithPageSize(List<BlueprintFile> blueprintFiles)
        {
            _logger.LogInformation($"Call to {nameof(ProcessWithPageSize)}");
            var tasks = new List<Task>();

            foreach (BlueprintFile blueprintFile in blueprintFiles)
            {
                tasks.Add(ProcessFile(blueprintFile));
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

        private async Task ProcessFile(BlueprintFile blueprintFile)
        {
            _logger.LogInformation($"Call to {nameof(ProcessFile)} with Id: {blueprintFile.Id}");

            // Delay 100ms to avoid blocking at the method "ReadImage"
            await Task.Delay(100);

            try
            {
                _logger.LogInformation($"Reading image {blueprintFile.FilePath} ...");
                FileStream fileStream = _imageService.ReadImage(blueprintFile.FilePath);
                if (fileStream != null)
                {
                    using (fileStream)
                    {
                        string fileExtension = blueprintFile.Extension.Contains(".") ? blueprintFile.Extension.ToLower() : $".{blueprintFile.Extension.ToLower()}";
                        (int height, int width) dimentions = await _imageService.GetImageDimentions(fileStream, fileExtension, CancellationToken.None);

                        _logger.LogInformation($"Saving dimentions info for ImgId: {blueprintFile.Id} - {blueprintFile.OriginalFileName} with width/height: {dimentions.width}/{dimentions.height} ...");
                        var connection = _sqlConnectionFactory.GetNewConnection();
                        const string updateQuery = @"UPDATE [dbo].[BlueprintFile] SET Width = @Width, Height=@Height WHERE Id = @Id";
                        await connection.ExecuteAsync(updateQuery, new { Width = dimentions.width, Height = dimentions.height, blueprintFile.Id });
                        connection.Dispose();
                        _logger.LogInformation($"Update successfully for ImgId: {blueprintFile.Id}.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed {nameof(ProcessFile)} with ImgId: {blueprintFile.Id}");
            }
        }
    }
}
