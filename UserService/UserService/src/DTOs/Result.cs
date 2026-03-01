namespace UserService.DTOs;

public class Result
{
    public bool IsSuccess { get; }
    public int StatusCode { get; }
    public string? ErrorMessage { get; }

    protected Result(bool isSuccess, int statusCode = 200, string errorMessage = "")
    {
        IsSuccess = isSuccess;
        StatusCode = statusCode;
        ErrorMessage = errorMessage;
    }

    public static Result Success(int statusCode = 200)
        => new(true, statusCode);

    public static Result Failure(int statusCode = 500, string errorMessage = "") =>
        new(false, statusCode, errorMessage);
}

public class Result<T> : Result
{
    public T? Value { get; }

    private Result(bool isSuccess, T? value, int statusCode = 200, string errorMessage = "") : base(isSuccess,
        statusCode, errorMessage)
    {
        Value = value;
    }

    public static Result<T> Success(T value, int statusCode = 200) => new(true, value, statusCode);

    public static Result<T> Failure(int statusCode = 200, string errorMessage = "") =>
        new(false, default, statusCode, errorMessage);
}