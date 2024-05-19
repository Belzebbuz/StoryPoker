using StoryPoker.Server.Abstractions.Room.Commands;
using StoryPoker.Server.Abstractions.Room.Models;

namespace StoryPoker.Server.Abstractions.Room;

public interface IRoomGrain : IGrainWithGuidKey
{
    Task<RoomStateResponse> GetAsync(Guid playerId);
    Task<ResponseState> InitStateAsync(RoomRequest.CreateRoom request);

    Task<ResponseState> ExecuteCommandAsync(RoomCommand command);
}
