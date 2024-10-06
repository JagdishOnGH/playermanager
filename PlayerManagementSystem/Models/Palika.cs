using System.ComponentModel.DataAnnotations;

namespace PlayerManagementSystem.Models
{
    public class Palika
{
   [Key]
    public int PalikaId {get;set;} 
    
    public string Name {get;set;}
 
    public ICollection<Teams> Teams { get; set; }

    public ICollection<Ward> Wards { get; set; }

    
}
}