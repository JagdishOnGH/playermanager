namespace PlayerManagementSystem.Models;

public class Ward
{
  public  Guid WardId { get; set; }
  public  string WardNo { get; set; }
  public  Guid MunicipalityId { get; set; }
  public  Municipality Municipality { get; set; }
}