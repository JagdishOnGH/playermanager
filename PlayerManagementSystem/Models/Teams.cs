using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayerManagementSystem.Models
{
    public class Teams
    {
        [Key]
        public int TeamId { get; set; }

        [Required]
        public string TeamName { get; set; }
        public TeamOf TeamOf { get; set; }
        public int AssociationId { get; set; }
        
        public ICollection<PersonalDetails> PersonalDetails { get; set; } = new List<PersonalDetails>();







    }

    public enum TeamOf
    {
        Ward,
        Palika,
    }
}