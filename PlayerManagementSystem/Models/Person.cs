using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayerManagementSystem.Models;

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

    public Roles Role {get;set;}
    public Gender Gender {get;set;}

    [ForeignKey("AddressP")]
    public int? PermanentAddress { get; set; }

    [ForeignKey("AddressT")]
    public int? TemporaryAddress { get; set; }

    public bool isSameAddress { get; set; }

    public virtual Address AddressP { get; set; }
    public virtual Address AddressT { get; set; }
    
}


public enum Gender{
    Male,
    Female,
    Others
}

public enum Roles{
    Player,
    Coach,
    Manager
}