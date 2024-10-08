using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace PlayerManagementSystem.Models;

public class Ward
{
    [Key]
    public Guid WardId { get; set; }
    [Range(minimum:1, maximum:100)]
    
    public int wardNo { get; set; }
    public bool isLoginAssigned {get;set;}
    
    public ICollection<Teams> wardTeams { get; set; }
    
    
}