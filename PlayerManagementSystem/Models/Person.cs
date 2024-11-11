namespace PlayerManagementSystem.Models;

public class Person
{
    //person id first name last name email address roleId (enum) teamId (foreign key)
    int PersonId { get; set; }  
    string FirstName { get; set; }
    string LastName { get; set; }
    string Email { get; set; }
    Role Role { get; set; }
    int TeamId { get; set; }
    Team Team { get; set; }
    
}
public enum Role
{
   Player = 1,
    Coach = 2,
    Manager = 3,
   
}