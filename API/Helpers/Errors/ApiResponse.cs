namespace API.Helpers.Errors;

public class ApiResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; }

    public ApiResponse(int statusCode, string message = null)
    {
        StatusCode = statusCode;
        Message = message?? GetDefaultMessage(statusCode);
    }

    private string GetDefaultMessage(int statusCode)
    {
        return statusCode switch
        {
            400 => "Wrong Request",
            401 => "Unauthorized",
            404 => "Not Found",
            405 => "Method doesn't exist",
            500 => "Internal server error",
            _ => "Something failed"
        };
    }
}
