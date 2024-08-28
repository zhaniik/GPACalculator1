using GPACalculator.Models; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace GPACalculator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly List<User> _users = new()
        {
            new User { Username = "user1", Password = "password1", Role = "Student" },
            new User { Username = "user2", Password = "password2", Role = "Teacher" },
            new User { Username = "user3", Password = "password3", Role = "Director"}
        };
        [HttpPost]
        [Route("Login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            var user = _users.FirstOrDefault(u =>
                u.Username == loginRequest.Username &&
                u.Password == loginRequest.Password);
            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }
            var token = GenerateJwtToken(user.Username, user.Role); 
            return Ok(new { Token = token });
        }
        private static string GenerateJwtToken(string username, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("your-secret-key-that-is-at-least-16-bytes-long");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role) 
        }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = "your-issuer",
                Audience = "your-audience"
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
