using Microsoft.AspNetCore.WebUtilities;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.WriteModels;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.SharedKernel;
using System.Collections.Generic;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.UploadFiles
{
    public class UploadResponse : BaseResponseObject
    {
        public IList<UploadResponseMessage> Messages { get; set; }

        public List<FileDto> Files { get; set; }

        public KeyValueAccumulator FormAccumulator { get; set; }
    }
}