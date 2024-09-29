using System.ComponentModel.DataAnnotations;

namespace PlayerManagementSystem.Models
{
    public class Palika
{
   [Key]
    public int PalikaId {get;set;} 
    [Required]
    public string Name {get;set;}

    public List<int> Wards {get;set;} = new List<int>();

    public List<int> LocalTeams {get;set;} = new List<int>();
}
}