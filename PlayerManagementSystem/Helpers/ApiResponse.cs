namespace PlayerManagementSystem.Helpers;

public class ApiResponse<T>
{
    public ApiResponse()
    {
        if(this.Error != null)
        {
            this.message = "Error";
        }
        else
        {
            this.message = "Success";
        }
        
    }
    public string message { get; set; } 
    public string? Error { get; set; } 
    
    public T? Data { get; set; }
}