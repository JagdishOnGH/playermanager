using System.ComponentModel.DataAnnotations;

namespace PlayerManagementSystem.Models
{
    public class PersonalDetails
    {
        [Key]
        public int Id { get; set; }      
        [MaxLength(250)]// Primary Key for PersonalDetails
        public string ProfilePicUrl { get; set; } // URL for Profile Picture
        
        public required string Name { get; set; }          // Full Name
        public string PhoneNo { get; set; }       // Contact Phone Number
        public string Email { get; set; }         // Email Address
        public DateOnly Dob { get; set; }  
        // Date of Birth
        public Gender Gender { get; set; } 
        public int RoleId { get; set; }           // Foreign Key for Role
       public virtual Role Role { get; set; } 
        
        
        
        public List<Address> Addresses { get; set; } = new List<Address>();
    }

  public  enum Gender
    {
        Male =1,
        Female=2,
        Other=3
        
    }
 
    
   
    
}

