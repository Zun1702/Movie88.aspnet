namespace Movie88.Application.HandlerResponse;

/// <summary>
/// Result pattern for service layer responses
/// Provides a consistent way to handle success and error states
/// </summary>
public class Result<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();

    public static Result<T> Success(T data, string message = "Operation successful")
    {
        return new Result<T>
        {
            IsSuccess = true,
            Message = message,
            StatusCode = 200,
            Data = data
        };
    }

    public static Result<T> Created(T data, string message = "Resource created successfully")
    {
        return new Result<T>
        {
            IsSuccess = true,
            Message = message,
            StatusCode = 201,
            Data = data
        };
    }

    public static Result<T> NotFound(string message = "Resource not found")
    {
        return new Result<T>
        {
            IsSuccess = false,
            Message = message,
            StatusCode = 404
        };
    }

    public static Result<T> BadRequest(string message = "Bad request")
    {
        return new Result<T>
        {
            IsSuccess = false,
            Message = message,
            StatusCode = 400
        };
    }

    public static Result<T> Error(string message = "An error occurred", int statusCode = 500)
    {
        return new Result<T>
        {
            IsSuccess = false,
            Message = message,
            StatusCode = statusCode
        };
    }

    public static Result<T> Failure(string message)
    {
        return new Result<T>
        {
            IsSuccess = false,
            Message = message,
            StatusCode = 400
        };
    }

    public static Result<T> InternalServerError(string message = "Internal server error")
    {
        return new Result<T>
        {
            IsSuccess = false,
            Message = message,
            StatusCode = 500
        };
    }
}
