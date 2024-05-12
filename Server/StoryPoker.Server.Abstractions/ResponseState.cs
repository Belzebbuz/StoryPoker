using ErrorOr;

namespace StoryPoker.Server.Abstractions;


[GenerateSerializer, Immutable]
public class ResponseState
{
    protected ResponseState(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    [Id(0)] public bool IsSuccess { get; init; }
    [Id(1)] public string? Error { get; init; }

    public static ResponseState Success() => new (true, null);
    public static ResponseState Fail(string error) => new (false, error);
    public static implicit operator Task<ResponseState>(ResponseState state) => Task.FromResult(state);
}

[GenerateSerializer, Immutable]
public class ResponseState<T> : ResponseState
{
    [Id(0)] public T? Value { get; init; }

    private ResponseState(T value, bool isSuccess, string? error)
        : base(isSuccess, error)
    {
        Value = value;
    }
    private ResponseState(bool isSuccess, string? error)
        : base(isSuccess, error)
    {
    }
    public static ResponseState<T> Success(T value) => new (value, true, null);
    public static new ResponseState<T> Fail(string error) => new (false, error);
    public static implicit operator Task<ResponseState<T>>(ResponseState<T> state) => Task.FromResult(state);
}
