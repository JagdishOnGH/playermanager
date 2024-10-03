﻿namespace PlayerManagementSystem.Models
{
    public class PersonalDetails
    {
        public int Id { get; set; }               // Primary Key for PersonalDetails
        public string ProfilePicUrl { get; set; } // URL for Profile Picture
        public string Name { get; set; }          // Full Name
        public string PhoneNo { get; set; }       // Contact Phone Number
        public string Email { get; set; }         // Email Address
        public DateOnly Dob { get; set; }  
        // Date of Birth
        public Gender Gender { get; set; } 
        public int RoleId { get; set; }           // Foreign Key for Role
        public Role Role { get; set; } 
        
        public Teams Team { get; set; }
        public int TeamId { get; set; }
        public List<Address> Addresses { get; set; } = new List<Address>();
    }

  public  enum Gender
    {
        Male =1,
        Female=2,
        Other=3
        
    }
 
    
   
    
}

