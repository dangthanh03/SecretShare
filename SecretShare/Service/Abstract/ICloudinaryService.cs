using SecretShare.Models.Domains;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace SecretShare.Models.Service.Abstract
{
    public interface ICloudinaryService
    {
        Task<Result<UploadedFile>> UploadFile(IFormFile file, string userId, bool autoDelete,bool PublicFile);
        Task<Result<UploadedText>> UploadText(string text, string userId, bool autoDelete,bool PublicText);
        
      
        Task<Result<string>> DeleteFile(string filePath);
    }

}
