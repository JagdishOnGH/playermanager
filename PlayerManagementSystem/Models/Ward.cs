using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayerManagementSystem.Models
{
    public sealed class Ward
    {
        [Key]
        public int WardId { get; set; }
        [ForeignKey("RefPalika")]
        public int PalikaId { get; set; }
       
        public List<int> Teams { get; set; } = new List<int>();

        public Palika RefPalika { get; set; }
        public ICollection<Teams> TeamList { get; set; } = new List<Teams>();
    }
}