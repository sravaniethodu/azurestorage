using AzureBlobStorage.WebApi.Models;
using AzureBlobStorage.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AzureBlobStorage.WebApi.Controllers
{


    
    [Route("api/[controller]")]
    [ApiController]
    public class StorageController : ControllerBase
    {
        private readonly IAzureStorage _storage;

        public StorageController(IAzureStorage storage)
        {
            _storage = storage;
        }
        /// <summary>
        ///  Get all files at the Azure Storage Location and return them
        /// </summary>
        /// <returns> Returns an empty array if no files are present at the storage container</returns>
        [HttpGet(nameof(Get))]
        public async Task<IActionResult> Get()
        {
            List<BlobDto>? files = await _storage.ListAsync();
            return StatusCode(StatusCodes.Status200OK, files);
        }
        /// <summary>
        /// upload file/files to storage
        /// </summary>
        /// <param name="file">files to be uploaded</param>
        /// <returns>return an error with details to the client/Return a success message to the client about successfull upload</returns>
        [HttpPost(nameof(Upload))]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            BlobResponseDto? response = await _storage.UploadAsync(file);
            if (response.Error == true)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response.Status);
            }
            else
            {
                return StatusCode(StatusCodes.Status200OK, response);
            }
        }
        /// <summary>
        /// download file from storage
        /// </summary>
        /// <param name="filename">filename which need to be downloaded</param>
        /// <returns>return error message to client/File was found, return it to client</returns>
        [HttpGet("{filename}")]
        public async Task<IActionResult> Download(string filename)
        {
            BlobDto? file = await _storage.DownloadAsync(filename);

            if (file == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"File {filename} could not be downloaded.");
            }
            else
            {
               // return Ok(file.Name);
                return File(file.Content, file.ContentType, file.Name);
            }
        }
        /// <summary>
        /// delete file from storage
        /// </summary>
        /// <param name="filename">filename to be deleted</param>
        /// <returns>error message to the client/ File has been successfully deleted message</returns>
        [HttpDelete("filename")]
        public async Task<IActionResult> Delete(string filename)
        {
            BlobResponseDto response = await _storage.DeleteAsync(filename);
            if (response.Error == true)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response.Status);
            } else
            {
                return StatusCode(StatusCodes.Status200OK, response.Status);
            }
        }
    }
}
