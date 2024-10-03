using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayerManagementSystem.Models
{
    public class Teams
    {
        [Key]
        public int TeamId { get; set; }

        public TeamOf TeamOf { get; set; } 

        public string? associationId { get; set; }
        
    }

    public enum TeamOf{
        ward ,
        palika
    }
}