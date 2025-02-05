namespace SavingsBook.Application.Contracts.Common;

public class ResponseDto<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public int StatusCode { get; set; }
    public T? Data { get; set; }

    public ResponseDto<T> SetSuccess(T data, int statusCode = 200, string? message = null)
    {
        Success = true;
        StatusCode = statusCode;
        Data = data;
        Message = message;
        return this;
    }

    // Optional: Helper to set failure with chaining
    public ResponseDto<T> SetFailure(int statusCode = 400, string? message = null)
    {
        Success = false;
        StatusCode = statusCode;
        Message = message;
        return this;
    }
    
}