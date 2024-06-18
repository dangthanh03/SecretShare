using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // Thêm namespace này để sử dụng Authorization attribute
using SecretShare.Models.Domains;
using SecretShare.Models.Service.Abstract;
using System.Threading.Tasks;
using System.Security.Claims;
using SecretShare.Service.Abstract;
using System.Net.Http.Headers; 
namespace SecretShare.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
  
    public class FilesController : ControllerBase

    {
        private readonly ApplicationDbContext _context;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IFileService _fileService;
        public FilesController(ApplicationDbContext context,ICloudinaryService cloudinaryService, IFileService  fileService)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
            _fileService = fileService;
        }

        // Add this attribute to use JWT authenticate
        [Authorize]
        [HttpPost("upload/file")]
        public async Task<IActionResult> UploadFile(IFormFile file, bool autoDelete = false, bool PublicFile = true) // Upload file
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Get the user ID from the token
            var result = await _cloudinaryService.UploadFile(file, userId, autoDelete,PublicFile); // Upload file to cloudinary
            if (result.IsSuccess)
            {
                var fileId = result.Data.Id;
                var fileUrl = Url.Action("GetFile", "Files", new { id = fileId }, Request.Scheme);
                return Ok(new { FileUrl = fileUrl }); // Return the url which wil lead to the GetFile action in controller
            }
            return BadRequest(new { Message = result.Message });
        }


        [Authorize]
        [HttpPost("upload/text")]
        public async Task<IActionResult> UploadText([FromBody] string text, bool autoDelete = false,bool PublicText=true) // Upload text
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Get the user ID from the token
            var result = await _cloudinaryService.UploadText(text, userId, autoDelete,PublicText);  // Upload file to cloudinary
            if (result.IsSuccess)
            {
                var textId = result.Data.Id;
                var textUrl = Url.Action("GetText", "Files", new { id = textId }, Request.Scheme);
                return Ok(new { TextUrl = textUrl }); // Return the url which wil lead to the GetText action in controller
            }
            return BadRequest(new { Message = result.Message });
        }

        
        [HttpGet("file/{id}")]
        public async Task<IActionResult> GetFile(string id) // Get file via id
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Get the user ID from the token
            var result = await _fileService.GetFileById(id,userId);
            if (!result.IsSuccess)
            {
                return NotFound(new { Message = result.Message });
            }
            if(result.Data.AutoDelete && result.Data.HasbeenDowloaded) // Check whether the file has been downloaded and has auto delete after dowloading
            {
                return BadRequest(new { Message = "File has been deleted"});

            }

            var file = result.Data;
            var filePath = file.FilePath; // The url leading to the file on cloudinary

            using (var httpClient = new HttpClient())
            {
                var fileStream = await httpClient.GetStreamAsync(filePath);
                using (var memoryStream = new MemoryStream())
                {
                    await fileStream.CopyToAsync(memoryStream);
                    var fileBytes = memoryStream.ToArray();
                    result.Data.HasbeenDowloaded = true;
                    _context.SaveChangesAsync(); // Update the HasbeenDowloaded column in database
                    if (file.AutoDelete)
                    {
                    
                        await _cloudinaryService.DeleteFile(filePath); //Delete file from cloudinary
                    }

                    return File(fileBytes, "application/octet-stream", Path.GetFileName(filePath));
                }
            }
        }
        [HttpGet("text/{id}")]
        public async Task<IActionResult> GetText(string id) //Get text via id
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Get the user ID from the token
            var result = await _fileService.GetTextById(id,userId);
            if (!result.IsSuccess)
            {
                return NotFound(new { Message = result.Message });
            }
            if (result.Data.AutoDelete && result.Data.HasbeenDowloaded) // Check whether the text has been downloaded and has auto delete after dowloading
            {
                return BadRequest(new { Message = "Text has been deleted" });

            }

            var file = result.Data;
            var filePath = file.FilePath;  // The url leading to the file on cloudinary

            using (var httpClient = new HttpClient())
            {
                var fileStream = await httpClient.GetStreamAsync(filePath);
                using (var memoryStream = new MemoryStream())
                {
                    await fileStream.CopyToAsync(memoryStream);
                    var fileBytes = memoryStream.ToArray();
                    result.Data.HasbeenDowloaded = true;
                    _context.SaveChangesAsync();  // Update the HasbeenDowloaded column in database
                    if (file.AutoDelete)
                    {

                        await _cloudinaryService.DeleteFile(filePath); //Delete file from cloudinary
                    }

                    return File(fileBytes, "application/octet-stream", Path.GetFileName(filePath));
                }
            }
        }

    
        [HttpGet("uploaded-files")]
        public async Task<IActionResult> GetAllUploadedFiles() // Get all of uploaded files, this action is for testing
        {
            var result = await _fileService.GetAllUploadedFiles();

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(new { Message = result.Message });
        }

      
        [HttpGet("uploaded-texts")]
        public async Task<IActionResult> GetAllUploadedTexts() // Get all of uploaded text, this action is for testing
        {
            var result = await _fileService.GetAllUploadedTexts();

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(new { Message = result.Message });
        }

        [Authorize]
        [HttpGet("user-files")]
        public async Task<IActionResult> GetUserFiles()  // Get all files which was uploaded by the logged-in user
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Get the user ID from the token
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "User not authorized" });
            }

            var result = await _fileService.GetFilesByUserId(userId);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(new { Message = result.Message });
        }

        [Authorize]
        [HttpGet("user-texts")]
        public async Task<IActionResult> GetUserTexts() // Get all texts which was uploaded by the logged-in user
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Get the user ID from the token
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "User not authorized" });
            }

            var result = await _fileService.GetTextsByUserId(userId);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(new { Message = result.Message });
        }

        [HttpGet("public-files/{userId}")]
        public async Task<IActionResult> GetPublicFilesByUserId(string userId) // Get public files from a specific user 
        {
            var result = await _fileService.GetPublicFilesByUserId(userId);

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(new { Message = result.Message });
        }

        [HttpGet("public-texts/{userId}")]
        public async Task<IActionResult> GetPublicTextsByUserId(string userId)  // Get public files texts a specific user 
        {
            var result = await _fileService.GetPublicTextsByUserId(userId);

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(new { Message = result.Message });
        }

    }
}
