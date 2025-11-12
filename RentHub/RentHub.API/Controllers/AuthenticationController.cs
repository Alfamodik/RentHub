using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RentHub.API.ResponceModels;
using RentHub.Core.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LoginRequest = RentHub.API.RequestModels.LoginRequest;

namespace RentHub.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        [HttpPost("email_exists")]
        public IActionResult EmailExists([FromForm] string email)
        {
            RentHubContext context = new();

            bool emailExists = context.Users
                .Any(u => u.Email == email);

            return Ok(new { exists = emailExists });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            using RentHubContext context = new();

            User? user = context.Users
                .FirstOrDefault(u => u.Email == request.Email);

            if (user == null)
                return Unauthorized();

            if (new PasswordHasher<User>().VerifyHashedPassword(null, user.Password, request.Password) != PasswordVerificationResult.Success)
                return Unauthorized();

            string token = GenerateJwtToken(user.UserId, user.Email);
            return Ok(new { token });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest();

            using RentHubContext context = new();

            User? user = context.Users.FirstOrDefault(u => u.Email == request.Email);

            if (user != null)
                return Conflict("Данный email уже зарегестрирован");

            user = new()
            {
                Email = request.Email,
                Password = new PasswordHasher<User>().HashPassword(null, request.Password)
            };

            context.Users.Add(user);
            context.SaveChanges();

            string token = GenerateJwtToken(user.UserId, request.Email);
            return Ok(new { token });
        }

        [Authorize]
        [HttpGet("has-avito-access")]
        public IActionResult UserHasAvitoAccess()
        {
            string? claimedUserid = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(claimedUserid, out int userId))
                return Unauthorized();

            RentHubContext context = new();
            User? user = context.Users.FirstOrDefault(user => user.UserId == userId);

            if (user == null)
                return Unauthorized();

            if (user.AvitoAccessToken != null && user.AvitoRefreshToken != null)
                return Ok(new HasAvitoAccessResponse() { HasAccess = true });

            return Ok(new HasAvitoAccessResponse() { HasAccess = false });
        }

        private static string GenerateJwtToken(int userId, string email)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email)
            };

            var filePath = Path.GetFullPath(
                Path.Combine("AppSettings", "appsettings.Development.json"));

            var config = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile(filePath, optional: true)
                    .Build();

            SymmetricSecurityKey symmetricSecurityKey = new(Encoding.UTF8.GetBytes(config["SymmetricSecurityKey"]!));
            SigningCredentials signingCredentials = new(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
                issuer: "RentHub",
                audience: "RentHubUsers",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: signingCredentials));
        }
    }
}
