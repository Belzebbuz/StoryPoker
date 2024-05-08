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

public class ResponseState<T> : ResponseState where T: class
{
    [Id(0)] public T? Value { get; init; }

    protected ResponseState(T? value, bool isSuccess, string? error)
        : base(isSuccess, error)
    {
        Value = value;
    }
    public static ResponseState Success(T value) => new ResponseState<T>(value, true, null);
    public static new ResponseState Fail(string error) => new ResponseState<T>(null,false, error);
}
