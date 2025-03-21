using MessMeApi.Entities.Models;
using MessMeApi.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MessMeApi.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<string> Authenticate(string username, string password)
        {
            User user = await _userRepository.GetUserByUsernameAsync(username);
            if (user == null || HashPassword(password, user.Salt) != user.PasswordHash)
                return null;

            return GenerateJwtToken(user);
        }

        public async Task<string> RegisterUser(string username, string password, string email, string role)
        {
            // Check if email already exists
            if (await _userRepository.EmailExists(email))
            {
                throw new Exception("Email already exists. Please use a different email.");
            }

            // Generate salt and hash the password
            string salt = GenerateSalt();
            string hashedPassword = HashPassword(password, salt);

            // Create new User object
            var newUser = new User
            {
                Username = username,
                PasswordHash = hashedPassword,
                Salt = salt,
                Email = email,
                Role = role
            };

            // Register the user in the database
            var isCreatedUser = await _userRepository.RegisterUser(newUser);
            if (isCreatedUser) 
            {
                return await Authenticate(username, password);
            }
            else
            {
                return "";
            }
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = issuer, // ✅ Setting Issuer
                Audience = audience, // ✅ Setting Audience
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        private static string HashPassword(string password, string salt)
        {
            using var sha256 = SHA256.Create();
            var combined = Encoding.UTF8.GetBytes(password + salt);
            return Convert.ToBase64String(sha256.ComputeHash(combined));
        }
    }
}
