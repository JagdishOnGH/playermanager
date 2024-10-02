namespace PlayerManagementSystem.Helpers;

public class ApiResponse<T>()
{
    public string message { get; set; } = "Success";
    public string? Error { get; set; } 
    
    public T? Data { get; set; }
}