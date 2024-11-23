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
public enum AuthRoles
{
    WardAdmin = 1,
    MunicipalityAdmin = 2,
    DistrictAdmin = 3,
    ProvinceAdmin = 4,
    SuperAdmin = 5
}
