using System.ComponentModel.DataAnnotations;

namespace SecretShare.Models.Domains
{
    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
