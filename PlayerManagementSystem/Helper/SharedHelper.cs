using PlayerManagementSystem.Helpers;

namespace PlayerManagementSystem.Helper;

public class SharedHelper
{
   public static  ApiResponse<string> CreateErrorResponse(string message)
    {
        return new ApiResponse<string> { Error = message };
    }
}