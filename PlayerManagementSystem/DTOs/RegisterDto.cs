using System.ComponentModel.DataAnnotations;
using PlayerManagementSystem.Models;

namespace PlayerManagementSystem.DTOs;

public class RegisterDto
{
    
    public string UserName { get; set; }
    [EmailAddress]
    public string Email { get; set; }
    
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    
   public TerritoryType Role { get; set; }
    public Guid TerritoryId  { get; set; }
    
}

