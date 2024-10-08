using System.ComponentModel.DataAnnotations;



namespace PlayerManagementSystem.Models.Auth;

public class RegisterPalikaCredentialDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character."
    )]
    public string Password { get; set; }

    public Guid? PalikaId { get; set; }
}