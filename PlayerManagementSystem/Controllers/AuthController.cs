using System.Security.Policy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using PlayerManagementSystem.EfContext;
using PlayerManagementSystem.Models;
using PlayerManagementSystem.Models.Auth;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using PlayerManagementSystem.Helpers;

namespace PlayerManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController :ControllerBase
    {
        public readonly EfDbContext _context;
        public readonly IConfiguration _config;

        public AuthController(EfDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            
            

            try{
                if (!ModelState.IsValid)
                {
                    // Extract the validation errors from the ModelState
                    var errors = ModelState
                        .Where(ms => ms.Value.Errors.Any())
                        .SelectMany(ms => ms.Value.Errors.Select(e => e.ErrorMessage))
                        .ToList();

                    // Return a custom error response with the extracted errors
                    return BadRequest(new ApiResponse<string>
                    {
                        Error = string.Join("; ", errors)  // Combine all errors into a single string
                    });
                }
                if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                {
                    return BadRequest(new { message = "Email already exists" });
                }

                var user = new User
                {
                    Email = registerDto.Email,
                    Role = registerDto.Role,
                    Password = HashPassword(registerDto.Password)
                };
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return Ok(new { message = "User registered successfully" });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        private string HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
        private bool VerifyPassword(string password, string hashedPassword)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == hashedPassword;
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["key"]);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(ms => ms.Value.Errors.Any())
                    .SelectMany(ms => ms.Value.Errors.Select(e => e.ErrorMessage))
                    .ToList();

                // Return a custom error response with the extracted errors
                return BadRequest(new ApiResponse<string>
                {
                    Error = string.Join("; ", errors)  // Combine all errors into a single string
                });
                
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user == null)
            {
                return BadRequest(new ApiResponse<string>{Error = "User not found"});
            }

            if (!VerifyPassword(loginDto.Password, user.Password))
            {
                return BadRequest(new ApiResponse<string>{Error = "Invalid Credentials"});
            }

            var token = GenerateJwtToken(user);
            return Ok(new ApiResponse<Dictionary<string,string>>{Data = new Dictionary<string, string>{{"token", token}}});
        }
        
            
        

    }
}