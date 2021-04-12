using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Configuration.ApplicationSettings;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.DeleteFile;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.DeleteFiles;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.GetFileById;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.GetMultipleFilesByIds;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.GetResizedFileById;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.ReadModels;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Application.Files.UploadFiles;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Controllers
{
    /// <summary>
    /// The FileController.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly AssetSettings _configuration;
        private readonly MimeTypeFactory _mimeTypeFactory;

        /// <summary>
        /// File Controller.
        /// </summary>
        /// <param name="mediator">The mediator.</param>
        /// <param name="config">The config.</param>
        /// <param name="mimeTypeFactory">Mime Type Factory.</param>
        public FileController(IMediator mediator,
            IOptions<AssetSettings> config,
            MimeTypeFactory mimeTypeFactory)
        {
            _mediator = mediator;
            _configuration = config.Value;
            _mimeTypeFactory = mimeTypeFactory;
        }

        /// <summary>
        /// Gets File By Id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Task{BaseFileDto}.</returns>
        [HttpGet("by-id/{id}")]
        public async Task<BaseFileDto> GetFileById(int id)
        {
            return await _mediator.Send(new GetFileByIdQuery(id));
        }

        /// <summary>
        /// Gets multiple files by ids.
        /// </summary>
        /// <param name="ids">ids</param>
        /// <returns>Task{IList{BaseFileDto}}.</returns>
        [HttpPost("multi")]
        public async Task<IList<BaseFileDto>> GetMultipleFilesByIds(IList<long> ids)
        {
            return await _mediator.Send(new GetMultipleFilesByIdsQuery(ids));
        }

        /// <summary>
        /// Deletes file.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Task{bool}.</returns>
        [HttpDelete("delete")]
        public async Task<bool> DeleteFile(int id)
        {
            await _mediator.Send(new DeleteFileCommand(id));
            return true;
        }

        /// <summary>
        /// Deletes multiple files.
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <returns>Task{bool}.</returns>
        [HttpPost("delete/multi")]
        public async Task<bool> DeleteFiles(IList<long> ids)
        {
            await _mediator.Send(new DeleteFilesCommand(ids));
            return true;
        }

        /// <summary>
        /// Uploads multiple files.
        /// </summary>
        /// <returns>Task{IActionResult}.</returns>
        [HttpPost("Upload")]
        [DisableFormValueModelBinding]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadFiles()
        {
            var result = new UploadResponse();
            UploadResponse uploadResponse = await Request.StreamFile(_configuration);
            if (uploadResponse != null)
            {
                result = await _mediator.Send(new UploadFilesCommand(uploadResponse));
            }

            return Ok(result);
        }

        /// <summary>
        /// Get Original image or resized image or throw exception.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="imgSize">The imgSize.</param>
        /// <returns>Task{IActionResult}.</returns>
        [HttpGet("get-resized-image/{id}")]
        public async Task<IActionResult> GetResizedImage(long id, string imgSize)
        {
            string physicalPath = await _mediator.Send(new GetResizedFileByIdQuery(id, imgSize));

            string fileExtension = Path.GetExtension(physicalPath);

            string mimeType = _mimeTypeFactory.GetMimeType(fileExtension);

            return PhysicalFile(physicalPath, mimeType);
        }

        /// <summary>
        /// Validates and resize image.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="imgSize">The imgSize.</param>
        /// <returns>Task{bool}.</returns>
        [HttpGet("resize-image/{id}")]
        public async Task<bool> ResizeImage(long id, string imgSize)
        {
            bool resizeResult = await _mediator.Send(new ResizeImageCommand(id, imgSize));
            return resizeResult;
        }
    }
}