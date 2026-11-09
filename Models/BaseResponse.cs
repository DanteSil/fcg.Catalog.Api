namespace Fcg.CatalogAPI.Models;

public sealed class BaseResponse
{
    public bool IsSuccess { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();
    public object? Data { get; init; }
    public DetailedErrorResponse? Error { get; init; }

    public static BaseResponse Success(object? data) => new()
    {
        IsSuccess = true,
        Data = data
    };

    public static BaseResponse Failure(int code, string message, IReadOnlyList<string>? errors = null)
    {
        var errorList = errors is { Count: > 0 }
            ? errors.ToList()
            : new List<string>();

        if (!string.IsNullOrWhiteSpace(message) && !errorList.Contains(message))
            errorList.Insert(0, message);

        return new BaseResponse
        {
            IsSuccess = false,
            Errors = errorList,
            Error = new DetailedErrorResponse
            {
                Message = message,
                Code = code
            }
        };
    }

    public sealed class DetailedErrorResponse
    {
        public string Message { get; init; } = string.Empty;
        public int Code { get; init; }
    }
}
