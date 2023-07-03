namespace Entities.Utilities.Exceptions;

public class AppException : Exception
{
    public AppException(int statusCode, string message, object value)
    {
        StatusCode = statusCode;
        Message = message;
        Value = value;
    }

    public AppException(int statusCode, object value)
    {
        StatusCode = statusCode;
        Value = value;
    }

    public AppException(int statusCode, string message)
    {
        StatusCode = statusCode;
        Message = message;
    }

    public AppException(int statusCode)
    {
        StatusCode = statusCode;
    }

    public int StatusCode { get; set; }
    public string Message { get; set; }
    public object Value { get; set; }
}