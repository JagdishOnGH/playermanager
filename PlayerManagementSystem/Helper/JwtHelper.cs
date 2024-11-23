using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PlayerManagementSystem.Models.AuthModel;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PlayerManagementSystem.Helper
{
    public class JwtHelper
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;

        public JwtHelper(IConfiguration configuration, UserManager<AppUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            
        }

        public  string GenerateJwt(AppUser user, string role)
        {
            List<Claim> allClaims =
            [
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sid, user.Id),
                new Claim("Role", role),
                new Claim("TerritoryId", user.TerritoryId.ToString())
            ];
            var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                allClaims,
                expires: DateTime.Now.AddDays(3),
                signingCredentials: credentials
            );
            var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
            return token;

           
        }
    }
}
