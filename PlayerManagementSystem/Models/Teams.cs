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

        public int? CoachId { get; set; }
        public Coach Coach { get; set; }
        public int? ManagerId { get; set; }
        public Manager Manager { get; set; }
        public List<Player> Players { get; set; }


      
        
    }
}