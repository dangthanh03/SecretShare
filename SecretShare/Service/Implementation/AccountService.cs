using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SecretShare.Models.Domains;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using SecretShare.Models.Service.Abstract;
using System.Security.Cryptography;

namespace SecretShare.Models.Service.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<Result<string>> RegisterAsync(RegisterModel model)
        {
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Result<string>.Success("Register success");
            }
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result<string>.Fail(errors);
        }

        public async Task<Result<LoginResponse>> LoginAsync(LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return Result<LoginResponse>.Fail("Unauthorized user");
            }

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, model.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["JWT:Key"] ?? throw new InvalidOperationException("Key not configured")));

            var token = new JwtSecurityToken(

                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                expires: DateTime.UtcNow.AddMinutes(4),
                claims: authClaims,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddMinutes(30);
            await _userManager.UpdateAsync(user);
            LoginResponse result = new LoginResponse
            {
                JwtToken = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo,
                RefreshToken = refreshToken
            };
            return Result<LoginResponse>.Success(result);
           
        }

      
        private async Task<JwtSecurityToken> GenerateJwt(string email)
        {
            var user = await  _userManager.FindByEmailAsync(email);

       
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Email,email),
                 new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
                
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["JWT:Key"] ?? throw new InvalidOperationException("Key not configured")
                ));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                expires: DateTime.UtcNow.AddMinutes(4),
                claims: authClaims,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256))
                ;
            return token;

        }

        private  ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var secret = _configuration["JWT:Key"] ?? throw new InvalidOperationException("Key not configured");

            var validation = new TokenValidationParameters
            {
                ValidIssuer = _configuration["JWT:Issuer"],
                ValidAudience = _configuration["JWT:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ValidateLifetime=false
            };

            return new JwtSecurityTokenHandler().ValidateToken(token, validation, out _);
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<Result<bool>> LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            return Result<bool>.Success(true);
        }

        public async Task<Result<LoginResponse>> Refresh(RefreshModel model)
        {
            var principal = GetPrincipalFromExpiredToken(model.AccessToken);
          
            
            if (principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value is null)
            {
                return Result<LoginResponse>.Fail("User can not be found");
            }

            var user = await _userManager.FindByEmailAsync(principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value);
            
           if(user is null || user.RefreshToken != model.RefreshToken || user.RefreshTokenExpiry< DateTime.UtcNow)
            {
                return Result<LoginResponse>.Fail("User is not authorized");
            }

            var token = await GenerateJwt(user.Email);

            LoginResponse result = new LoginResponse
            {
                JwtToken = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo,
                RefreshToken = model.RefreshToken

            };

            return Result<LoginResponse>.Success(result);

        }
    }
}
