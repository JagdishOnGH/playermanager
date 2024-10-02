using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayerManagementSystem.Models
{
    public sealed class Ward
    {
        [Key]
        public int WardId { get; set; }
        
        public int PalikaId { get; set; }
       
        public List<int> Teams { get; set; } = new List<int>();

        public Palika Palika { get; set; }
        public ICollection<Teams> TeamList { get; set; } = new List<Teams>();
    }
}