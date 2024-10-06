using System.ComponentModel.DataAnnotations;

namespace PlayerManagementSystem.Models.Auth
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }

        [Required]
        public Userrole Role { get; set; }
    }
}