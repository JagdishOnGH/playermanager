// Models/Auth/RegisterDto.cs
using System.ComponentModel.DataAnnotations;
using PlayerManagementSystem.Models;

namespace PlayerManagementSystem.Models.Auth
{
    public class RegisterDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        public Role Role { get; set; }
    }
}
