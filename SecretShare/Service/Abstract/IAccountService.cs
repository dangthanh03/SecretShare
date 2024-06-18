using SecretShare.Models.Domains;
using System.Threading.Tasks;

namespace SecretShare.Models.Service.Abstract
{
    public interface IAccountService
    {
        Task<Result<string>> RegisterAsync(RegisterModel model);
        Task<Result<string>> LoginAsync(LoginModel model);
        Task<Result<bool>> LogoutAsync();
    }

}
