namespace PlayerManagementSystem.Helpers;

public class ApiResponse<T>
{
    private string? _error;

    public string message => !string.IsNullOrEmpty(Error) ? "Error" : "Success";

    public string? Error
    {
        get => _error;
        set
        {
            _error = value;
            // You can also update other related properties here if needed
        }
    }
    
    public T? Data { get; set; }
}
