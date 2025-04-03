namespace Questao5.Domain.Errors;
public class Result<T>
{
    public bool Success { get; }
    public T Data { get; }
    public string ErrorType { get; }
    public string ErrorMessage { get; }

    private Result(bool success, T data, string errorType, string errorMessage)
    {
        Success = success;
        Data = data;
        ErrorType = errorType;
        ErrorMessage = errorMessage;
    }

    public static Result<T> Ok(T data) => new(true, data, null!, null!);
    public static Result<T> Fail(string errorType, string errorMessage) => new Result<T>(false, default!, errorType, errorMessage);
}