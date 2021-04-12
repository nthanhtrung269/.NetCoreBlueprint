using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.ApplicationSettings;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.WriteModels;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Enums;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.UploadFiles
{
    public static class FileStreamingHelper
    {
        private static readonly FormOptions _defaultFormOptions = new FormOptions()
        {
        };

        public static async Task<UploadResponse> StreamFile(this HttpRequest request, AssetSettings assetSettings)
        {
            Guard.AgainstInvalidArgument(nameof(request.ContentType), MultipartRequestHelper.IsMultipartContentType(request.ContentType));

            var results = new UploadResponse();
            results.Messages = new List<UploadResponseMessage>();
            var formAccumulator = new KeyValueAccumulator();

            // Used to accumulate all the form url encoded key value pairs in the request.
            var fileDtos = new List<FileDto>();
            var boundary = MultipartRequestHelper.GetBoundary(
                 MediaTypeHeaderValue.Parse(request.ContentType),
                 _defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, request.Body);

            var section = await reader.ReadNextSectionAsync();

            while (section != null)
            {
                ContentDispositionHeaderValue contentDisposition;
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out contentDisposition);

                if (hasContentDispositionHeader)
                {
                    string orginalFileName = contentDisposition.FileName.Value;
                    string fileName = $"{DateTime.Now:yyyyMMddHHmmss}_{orginalFileName}";
                    string targetFile = Path.Combine(assetSettings.TempDataFolder, fileName);

                    if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                    {
                        using (var memoryStream = File.Create(targetFile))
                        {
                            await section.Body.CopyToAsync(memoryStream);

                            if (memoryStream.Length == 0)
                            {
                                results.Messages.Add(new UploadResponseMessage()
                                {
                                    Text = $"The file {orginalFileName} is empty.",
                                    Code = "FileEmpty"
                                });

                            }
                            else if (memoryStream.Length > assetSettings.FileSizeLimit)
                            {
                                var megabyteSizeLimit = assetSettings.FileSizeLimit / 1048576; //1MB
                                results.Messages.Add(new UploadResponseMessage()
                                {
                                    Text = $"The file {orginalFileName} exceeds limit {megabyteSizeLimit:N1} MB.",
                                    Code = "ExceedLimit"
                                });
                            }
                            else
                            {
                                string extension = Path.GetExtension(targetFile);

                                var fileDto = new FileDto()
                                {
                                    OriginalFileName = orginalFileName,
                                    FilePath = targetFile,
                                    Extension = extension != null ? extension.Replace(".", "") : "",
                                    FileName = fileName
                                };

                                // Copy stream to dto would be faster then re-read file from physical disk
                                fileDto.StreamData = new MemoryStream();
                                memoryStream.Position = 0;
                                await memoryStream.CopyToAsync(fileDto.StreamData);

                                fileDtos.Add(fileDto);
                            }
                        }
                    }
                    else if (MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition))
                    {
                        // Don't limit the key name length because the multipart headers length limit is already in effect.
                        var key = HeaderUtilities
                            .RemoveQuotes(contentDisposition.Name).Value;
                        var encoding = GetEncoding(section);

                        using (var streamReader = new StreamReader(
                            section.Body,
                            encoding,
                            detectEncodingFromByteOrderMarks: true,
                            bufferSize: 1024,
                            leaveOpen: true))
                        {
                            // The value length limit is enforced by MultipartBodyLengthLimit
                            var value = await streamReader.ReadToEndAsync();

                            if (string.Equals(value, "undefined",
                                StringComparison.OrdinalIgnoreCase))
                            {
                                value = string.Empty;
                            }
                            formAccumulator.Append(key, value);
                        }
                    }
                }

                // Drains any remaining section body that has not been consumed and
                // reads the headers for the next section.
                section = await reader.ReadNextSectionAsync();
            }

            results.Files = fileDtos;
            results.FormAccumulator = formAccumulator;
            results.Status = true;
            results.ErrorCode = ResponseErrorCode.None;
            results.Message = "Ok";
            return results;
        }

        private static Encoding GetEncoding(MultipartSection section)
        {
            MediaTypeHeaderValue mediaType;
            var hasMediaTypeHeader = MediaTypeHeaderValue.TryParse(section.ContentType, out mediaType);

            // UTF-7 is insecure and should not be honored. UTF-8 will succeed in most cases.
            if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
            {
                return Encoding.UTF8;
            }
            return mediaType.Encoding;
        }
    }
}
