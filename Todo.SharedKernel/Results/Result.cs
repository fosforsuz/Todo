namespace Todo.SharedKernel.Results;

public class Result
{
    protected Result(bool isSuccess, string? message = null, List<string>? errorCodes = null,
        List<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Message = message;
        ErrorCodes = errorCodes ?? [];
        Errors = errors ?? [];
    }

    public Result()
    {
        ErrorCodes = new List<string>();
        Errors = new List<string>();
    }

    public bool IsSuccess { get; protected set; }
    public string? Message { get; protected set; }
    private List<string> ErrorCodes { get; }
    private List<string> Errors { get; }
    public bool HasError => ErrorCodes.Count > 0 || Errors.Count > 0;

    public void AddErrorCode(string errorCode)
    {
        ErrorCodes.Add(errorCode);
    }

    public void AddErrorCodes(List<string> errorCodes)
    {
        ErrorCodes.AddRange(errorCodes);
    }

    public void AddError(string error)
    {
        Errors.Add(error);
    }

    public void AddError(string error, string errorCode)
    {
        Errors.Add(error);
        ErrorCodes.Add(errorCode);
    }

    public void AddErrors(List<string> errors)
    {
        Errors.AddRange(errors);
    }

    public static Result Ok(string message = "Success")
    {
        return new Result(true, message);
    }

    public static Result Fail(string? message, string? errorCode, string? errorMessage)
    {
        return new Result(false, message, [errorCode ?? string.Empty],
            [errorMessage ?? string.Empty]);
    }

    public static Result Fail(string message)
    {
        return new Result(false, message);
    }

    public static Result Fail(string message, List<string> errorCodes, List<string> errors)
    {
        return new Result(false, message, errorCodes, errors);
    }

    public List<string> GetErrors()
    {
        return Errors;
    }

    public List<string> GetErrorCodes()
    {
        return ErrorCodes;
    }
}

public class Result<T> : Result
{
    public Result(T? value, bool isSuccess, string? message = null, List<string>? errorCodes = null,
        List<string>? errors = null)
        : base(isSuccess, message, errorCodes, errors)
    {
        Value = value;
    }

    public T? Value { get; private set; }

    public static Result<T> Ok(T value, string? message = null)
    {
        return new Result<T>(value, true, message);
    }

    public static Result<T> Fail(string message)
    {
        return new Result<T>(isSuccess: false, value: default, message: message, errorCodes: null, errors: null);
    }

    public static Result<T> Fail(List<string> errors)
    {
        return new Result<T>(default, false, errors: errors);
    }

    public static Result<T> Fail(string? message, List<string>? errorCodes, List<string>? errors)
    {
        return new Result<T>(default, false, message, errorCodes,
            errors);
    }
}