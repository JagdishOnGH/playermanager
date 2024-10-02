using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayerManagementSystem.Models
{
    public class Person
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }

        [Phone]
        public string Phone { get; set; }

        public Roles Role { get; set; }

        public Gender Gender { get; set; }

        // Renaming Foreign Key Properties to avoid conflicts with navigation properties
        [ForeignKey("PermanentAddressId")]
        public int? PermanentAddressId { get; set; }

        

        public bool isSameAddress { get; set; }

        // Navigation properties
        public virtual Address PermanentAddress { get; set; }

        

       
    }

    public enum Gender
    {
        Male,
        Female,
        Others
    }

    public enum Roles
    {
        Player,
        Coach,
        Manager
    }
}