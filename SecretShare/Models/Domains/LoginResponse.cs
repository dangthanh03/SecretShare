namespace SecretShare.Models.Domains
{
    public class LoginResponse
    {
        public string JwtToken { get; set; }
        public DateTime Expiration { get; set; }

        public required string RefreshToken { get; set; }
    }
}
