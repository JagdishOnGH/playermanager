namespace PlayerManagementSystem.Helpers;

public class ApiResponse<T>
{
    private T? _error;

    public string message => !string.IsNullOrEmpty(Error.ToString()) ? "Error" : "Success";

    public T? Error
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
