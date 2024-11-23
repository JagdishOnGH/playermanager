namespace PlayerManagementSystem.Models;

public class District
{
    public Guid DistrictId { get; set; }
    public string Name { get; set; }
    public Guid ProvinceId { get; set; }
    public Province Province { get; set; }
    
}