﻿namespace PlayerManagementSystem.Models;

public class Person
{
    //person id first name last name email address roleId (enum) teamId (foreign key)
  public  int PersonId { get; set; }  
   public string FirstName { get; set; }
   public string LastName { get; set; }
   public string Email { get; set; }
   public Role Role { get; set; }
   public int TeamId { get; set; }
   public Team Team { get; set; }
    
}
public enum Role
{
   Player = 1,
    Coach = 2,
    Manager = 3,
   
}