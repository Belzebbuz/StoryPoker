using StoryPoker.Server.Abstractions.Metadata.Models;

namespace StoryPoker.Server.Abstractions.Common;

public interface IRoomGrain
{
    Task<ResponseState> InitStateAsync(CreateRoomRequest request);
}

public interface IRoomGrain<in TCommand, TStateResponse> : IGrainWithGuidKey, IRoomGrain
    where TCommand : IRoomCommand
    where TStateResponse: IRoomStateResponse
{
    Task<TStateResponse> GetAsync(Guid playerId);
    Task<ResponseState> ExecuteCommandAsync(TCommand command);
}
