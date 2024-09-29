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

        [ForeignKey("Coach")]
        public int CoachId { get; set; }

        [ForeignKey("Manager")]
        public int ManagerId { get; set; }


        public List<int> Players { get; set; } = new List<int>();

        [ForeignKey("Ward")]
        public int? WardId { get; set; }

        [ForeignKey("Palika")]
        public int? PalikaId { get; set; }


        public virtual Person Coach { get; set; }
        public virtual Person Manager { get; set; }
        public virtual Ward Ward { get; set; }
        public virtual Palika Palika { get; set; }
        
    }
}