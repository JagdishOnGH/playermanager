namespace PlayerManagementSystem.Models;

public class Team
{
   public Guid TeamId { get; set; }
   public string Name { get; set; }
   public TerritoryType TerritoryType { get; set; }
   public Guid TerritoryId { get; set; } 
}
public enum TerritoryType
{
   Province = 4,
   Municipality = 2,
   Ward = 1,
   District = 3
}