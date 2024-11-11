namespace PlayerManagementSystem.Models;

public class Municipality
{
    int MunicipalityId { get; set; }
    string Name { get; set; }
    int DistrictId { get; set; }
    District District { get; set; }
}