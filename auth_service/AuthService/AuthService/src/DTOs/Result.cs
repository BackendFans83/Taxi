namespace AuthService.DTOs;

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Value { get; private set; }
    public int StatusCode { get; private set; }
    public string? ErrorMessage { get; private set; }

    public static Result<T> Success(T value, int statusCode = 200)
    {
        return new Result<T>
        {
            IsSuccess = true,
            Value = value,
            StatusCode = statusCode,
        };
    }
    
    public static Result<T> Error(int statusCode = 200, string errorMessage = "")
    {
        return new Result<T>
        {
            IsSuccess = false,
            ErrorMessage =  errorMessage,
            StatusCode = statusCode,
        };
    }
}