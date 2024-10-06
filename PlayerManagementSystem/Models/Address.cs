using System.ComponentModel.DataAnnotations;

namespace PlayerManagementSystem.Models
{
    public class Address{
        [Key]
        public int AddressId {get;set;}

        [Required]
        public string Tole {get;set;}

        [Range(1,100)]
        public int Ward {get;set;}
        [Required]
        public string Palika {get;set;}

        [Required]
        public string District {get;set;}

        [Required]
        public string State {get;set;}

        [Required]
        public string Country {get;set;}

        public bool IsPermanent { get; set; } = true;
        
        //public virtual ICollection<PersonalDetails> PersonalDetails { get; set; } = new List<PersonalDetails>();



    }
}