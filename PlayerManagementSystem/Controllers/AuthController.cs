using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PlayerManagementSystem.EfContext;
using PlayerManagementSystem.Models;
using PlayerManagementSystem.Models.Auth;
using PlayerManagementSystem.Helpers;

namespace PlayerManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly EfDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(EfDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // Register Endpoint
        [HttpPost("/palika/register")]
        public async Task<IActionResult> RegisterPalika([FromBody] RegisterPalikaDto registerDto)
        {
            try
            {
              
                var allPalikasQuery = _context.Palikas.Where(p=> !p.isLoginAssigned).AsQueryable();
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(ms => ms.Value != null && ms.Value.Errors.Any())
                        .SelectMany(ms => ms.Value?.Errors.Select(e => e.ErrorMessage) ?? Array.Empty<string>())
                        .ToList();

                    return BadRequest(new ApiResponse<string>
                    {
                        Error = string.Join("; ", errors)
                    });
                }
                
                if (registerDto.PalikaId != null && !await allPalikasQuery.AnyAsync(p => p.PalikaId == registerDto.PalikaId))
                {
                    return BadRequest(new ApiResponse<string> { Error = "Palika not found or already assigned " });
                }
               

                if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                {
                    return BadRequest(new ApiResponse<string> { Error = "User already exists" });
                }
                if(registerDto.PalikaId != null && !await _context.Palikas.AnyAsync(p => p.PalikaId == registerDto.PalikaId))
                {
                    return BadRequest(new ApiResponse<string> { Error = "Palika not found" });
                }

                var user = new User
                {
                    Email = registerDto.Email,
                    Password = HashPassword(registerDto.Password),
                    PalikaId = registerDto.PalikaId,  // Nullable
                         // Nullable
                };
                if(registerDto.PalikaId != null)
                {
                    var palika = await _context.Palikas.FirstOrDefaultAsync(p => p.PalikaId == registerDto.PalikaId);
                    palika!.isLoginAssigned = true;
                    _context.Palikas.Update(palika);
                }
                
                
                

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                return Ok(new { message = "User registered successfully", role = user.Role });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
        
        
        // Login Endpoint
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(ms => ms.Value.Errors.Any())
                    .SelectMany(ms => ms.Value.Errors.Select(e => e.ErrorMessage))
                    .ToList();

                return BadRequest(new ApiResponse<string>
                {
                    Error = string.Join("; ", errors)
                });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user == null)
            {
                return BadRequest(new ApiResponse<string> { Error = "User not found" });
            }

            if (!VerifyPassword(loginDto.Password, user.Password))
            {
                return BadRequest(new ApiResponse<string> { Error = "Invalid credentials" });
            }

            var token = GenerateJwtToken(user);
            return Ok(new ApiResponse<Dictionary<string, string>> 
            {
                Data = new Dictionary<string, string> 
                {
                    { "token", token },
                    { "role", user.Role }
                }
            });
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
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
