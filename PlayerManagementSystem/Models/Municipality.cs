namespace PlayerManagementSystem.Models;

public class Municipality
{
    public Guid MunicipalityId { get; set; }
    public string Name { get; set; }
    public Guid DistrictId { get; set; }
    public District District { get; set; }
}