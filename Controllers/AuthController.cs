using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PatientKiosk.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PatientKiosk.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private const string StaticUsername = "Kiosk";
        private const string StaticPassword = "Kiosk123";

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate static credentials
            if (!string.Equals(request.Username, StaticUsername, StringComparison.Ordinal) ||
                !string.Equals(request.Password, StaticPassword, StringComparison.Ordinal))
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            // Read JWT settings from configuration
            var key = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var expiryHoursValue = _configuration["Jwt:ExpiryHours"];

            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            {
                // Misconfiguration should be discovered during deployment; return 500 to indicate server config issue
                return StatusCode(500, new { message = "JWT configuration is invalid" });
            }

            if (!int.TryParse(expiryHoursValue, out var expiryHours))
            {
                expiryHours = 24; // fallback
            }

            var expiresAt = DateTime.UtcNow.AddHours(expiryHours);

            var claims = new List<Claim>
            {
                new Claim("username", request.Username)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(tokenDescriptor);

            var response = new LoginResponse
            {
                Token = tokenString,
                ExpiresAtUtc = expiresAt,
                Username = request.Username
            };

            return Ok(response);
        }
    }
}