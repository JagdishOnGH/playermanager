namespace PlayerManagementSystem.Models;

public class PersonTeam
{
    public Guid PersonId { get; set; }
    public Person Person { get; set; }
    public Guid TeamId { get; set; }
    public Team Team { get; set; }
}