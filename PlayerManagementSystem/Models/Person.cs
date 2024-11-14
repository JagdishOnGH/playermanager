namespace PlayerManagementSystem.Models;

public class Person
{
    //person id first name last name email address roleId (enum) teamId (foreign key)
  public  Guid PersonId { get; set; }  
   public string FirstName { get; set; }
   public string LastName { get; set; }
   public string Email { get; set; }
   public Role Role { get; set; }
   public Guid TeamId { get; set; }
   public Team Team { get; set; }
   
   public List<PersonTeam> PersonTeams { get; set; }
    
}
public enum Role
{
   Player = 1,
    Coach = 2,
    Manager = 3,
   
}