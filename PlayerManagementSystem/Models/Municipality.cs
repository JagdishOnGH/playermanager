namespace PlayerManagementSystem.Models;

public class Municipality
{
    public int MunicipalityId { get; set; }
    public string Name { get; set; }
    public int DistrictId { get; set; }
    public District District { get; set; }
}