namespace PlayerManagementSystem.Models;

public class District
{
    public int DistrictId { get; set; }
    public string Name { get; set; }
    public int ProvinceId { get; set; }
    public Province Province { get; set; }
    
}