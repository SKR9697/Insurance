using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using UserService.Models;
using UserService.Repositories;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _repo;
        private readonly IConfiguration _cfg;
        public AuthController(IUserRepository repo, IConfiguration cfg) { _repo = repo; _cfg = cfg; }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var existing = await _repo.GetByEmailAsync(dto.Email);
            if (existing is not null) return BadRequest("Email already registered");
            var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var id = await _repo.CreateAsync(dto.Email, hash);
            return Ok(new { id, email = dto.Email });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _repo.GetByEmailAsync(dto.Email);
            if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized();

            // Create JWT and get expiry
            var token = CreateJwt(user);
            var expiresIn = 3600; // Example: 1 hour in seconds. Adjust based on your JWT config.

            var response = new
            {
                token,
                expiresIn,
                user = new
                {
                    id = user.Id.ToString(),
                    email = user.Email
                }
            };

            return Ok(response);
        }


        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var sub = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(JwtRegisteredClaimNames.Email);
            var role = User.FindFirstValue(ClaimTypes.Role);
            return Ok(new { userId = sub, email, role });
        }

        private string CreateJwt(User user)
        {
            try
            {
                var jwt = _cfg.GetSection("Jwt");
                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwt["Key"]!));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var claims = new[] {
              new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
              new Claim(JwtRegisteredClaimNames.Email, user.Email),
              new Claim(ClaimTypes.Role, user.Role)
            };
                var token = new JwtSecurityToken(
                  issuer: jwt["Issuer"], audience: jwt["Audience"],
                  claims: claims, expires: DateTime.UtcNow.AddHours(8), signingCredentials: creds);
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
