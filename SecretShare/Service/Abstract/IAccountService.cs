using SecretShare.Models.Domains;
using System.Threading.Tasks;

namespace SecretShare.Models.Service.Abstract
{
    public interface IAccountService
    {
        Task<Result<string>> RegisterAsync(RegisterModel model);
        Task<Result<LoginResponse>> LoginAsync(LoginModel model);
        Task<Result<bool>> LogoutAsync();
        Task<Result<LoginResponse>> Refresh(RefreshModel model);
    }

}
