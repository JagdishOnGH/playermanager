using System.ComponentModel.DataAnnotations;

namespace PlayerManagementSystem.Models
{
    public class PersonalDetails
    {
        [Key]
        public int Id { get; set; }               // Primary Key for PersonalDetails
        public Address? Address { get; set; } 

        [Required]
        public string? FirstName { get; set; }          // Full Name
        public string? LastName { get; set; }       // Contact Phone Number
        public string? Email { get; set; }         
        public string? Phone { get; set; }         // Contact Phone Number
        public DateOnly Dob { get; set; }         // Date of Birth
        public Gender Gender { get; set; }        // Gender

        // List of Addresses referencing the Address class
        public List<Address> Addresses { get; set; } = new List<Address>(); 

        public Teams? Team { get; set; } 
        public int TeamId { get; set; }
        public int RoleId { get; set; }
        public Roles? Role { get; set; }
    }

   public  enum Gender{
        male,
        female,
        other
    }
    
}

