namespace PlayerManagementSystem.Helpers;

public class ApiResponse<T>
{
    

    public string? Message => !string.IsNullOrEmpty(Error?.ToString()) ? "Error" : "Success";

    public T? Error
    {
        get;
        set;
    }
    
    public T? Data { get; set; }
}
