using Microsoft.AspNetCore.Identity;

namespace SecretShare.Models.Domains
{
    public class ApplicationUser : IdentityUser
    {
        public string? RefreshToken { get; set; }

        public DateTime RefreshTokenExpiry { get; set; }
    }
}
