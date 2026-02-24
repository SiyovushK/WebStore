using Domain.Constants;

namespace Domain.Common;

public class Result<TValue>
{
    public bool IsSuccess { get; }
    public TValue? Value { get; }
    public ErrorDetails? Error { get; }

    private Result(TValue value) => (IsSuccess, Value, Error) = (true, value, null);
    private Result(ErrorDetails error) => (IsSuccess, Value, Error) = (false, default, error);

    public static Result<TValue> Success(TValue value) => new(value);
    public static Result<TValue> Failure(ErrorDetails error) => new(error);
}