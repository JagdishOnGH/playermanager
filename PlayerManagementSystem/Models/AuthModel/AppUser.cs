using Microsoft.AspNetCore.Identity;

namespace PlayerManagementSystem.Models.AuthModel;

public class AppUser: IdentityUser
{
  public  Guid TerritoryId { get; set; }

}
