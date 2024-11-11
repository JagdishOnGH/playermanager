namespace PlayerManagementSystem.Models;

public class Ward
{
    int WardId { get; set; }
    string Name { get; set; }
    int MunicipalityId { get; set; }
    Municipality Municipality { get; set; }
}