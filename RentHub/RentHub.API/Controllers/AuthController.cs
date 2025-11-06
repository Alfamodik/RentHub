using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Playwright;
using RentHub.Core.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LoginRequest = RentHub.API.RequestModels.LoginRequest;

namespace RentHub.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
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
            RentHubContext context = new();

            User? user = context.Users
                .FirstOrDefault(u => u.Email == request.Email && u.Password == request.Password);

            if (user == null)
                return Unauthorized();

            string token = GenerateJwtToken(request.Email);
            return Ok(new { token });
        }

        [HttpPost("Register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            RentHubContext context = new();

            User? user = context.Users.FirstOrDefault(u => u.Email == request.Email);

            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest();

            if (user != null)
                return Conflict("Данный email уже зарегестрирован");

            context.Users.Add(new()
            {
                Email = request.Email,
                Password = request.Password
            });

            context.SaveChanges();

            string token = GenerateJwtToken(request.Email);
            return Ok(new { token });
        }

        private static string GenerateJwtToken(string email)
        {
            var claims = new[]
            {
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
                issuer: "",
                audience: "",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: signingCredentials));
        }
    }
}
