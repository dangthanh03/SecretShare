using SecretShare.Models.Domains;
using SecretShare.Models.ViewModel;

namespace SecretShare.Service.Abstract
{
    public interface IFileService
    {
        Task<Result<UploadedText>> GetTextById(string id);
        Task<Result<UploadedFile>> GetFileById(string id);
        Task<Result<List<UploadFileVm>>> GetAllUploadedFiles();
        Task<Result<List<UploadTextVm>>> GetAllUploadedTexts();
        Task<Result<List<UploadFileVm>>> GetFilesByUserId(string userId);
        Task<Result<List<UploadTextVm>>> GetTextsByUserId(string userId);
    }
}
