using SecretShare.Models.Domains;
using SecretShare.Models.ViewModel;

namespace SecretShare.Service.Abstract
{
    public interface IFileService
    {
        Task<Result<UploadedText>> GetTextById(string id, string userId);
        Task<Result<UploadedFile>> GetFileById(string id, string userId);
        Task<Result<List<UploadFileVm>>> GetAllUploadedFiles();
        Task<Result<List<UploadTextVm>>> GetAllUploadedTexts();
        Task<Result<List<UploadFileVm>>> GetFilesByUserId(string userId);
        Task<Result<List<UploadTextVm>>> GetTextsByUserId(string userId);
        Task<Result<List<UploadFileVm>>> GetPublicFilesByUserId(string userId);
        Task<Result<List<UploadTextVm>>> GetPublicTextsByUserId(string userId);
    }
}
