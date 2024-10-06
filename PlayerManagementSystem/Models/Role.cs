using System.ComponentModel.DataAnnotations;

namespace PlayerManagementSystem.Models;

public class Role
{
    public int RoleId { get; set; }
    [RegularExpression("Manager|Player|Coach", ErrorMessage = "RoleName must be either 'Manager', 'Player', or 'Coach'")]
    public string RoleName { get; set; }    
    
}