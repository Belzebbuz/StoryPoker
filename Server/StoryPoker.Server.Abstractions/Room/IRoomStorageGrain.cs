using StoryPoker.Server.Abstractions.Room.Models;

namespace StoryPoker.Server.Abstractions.Room;

public interface IRoomStorageGrain : IGrainWithGuidKey
{
    Task<ResponseState<Guid>> CreateRoomAsync(RoomRequest.CreateRoom request);
    Task<bool> RoomExistAsync(Guid roomId);
}
