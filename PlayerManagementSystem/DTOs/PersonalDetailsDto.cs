using System.ComponentModel.DataAnnotations;
using PlayerManagementSystem.Models;

namespace PlayerManagementSystem.DTOs;



public class PersonDetailsDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
  
    
    public Role Role { get; set; }
}