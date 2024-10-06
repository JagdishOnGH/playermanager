using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace PlayerManagementSystem.Models;

public class Ward
{
    [Key]
    public Guid Id { get; set; }
    public int wardNo { get; set; }
    
    public ICollection<Teams> wardTeams { get; set; }
    
    
}