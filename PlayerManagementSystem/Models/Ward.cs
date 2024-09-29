using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayerManagementSystem.Models
{
    public class Ward
    {
        [Key]
        public int WardId { get; set; }
        [ForeignKey("Palikas")]
        public string Palika { get; set; }

        public List<int> Teams { get; set; } = new List<int>();

        public virtual Palika Palikas { get; set; }
    }
}