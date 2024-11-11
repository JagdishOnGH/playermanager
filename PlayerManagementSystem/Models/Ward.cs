namespace PlayerManagementSystem.Models;

public class Ward
{
  public  int WardId { get; set; }
  public  string Name { get; set; }
  public  int MunicipalityId { get; set; }
  public  Municipality Municipality { get; set; }
}