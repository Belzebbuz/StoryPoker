using StoryPoker.Server.Abstractions.Room.Models;
using StoryPoker.Server.Grains.RoomGrains.Models;

namespace StoryPoker.Server.Grains.Abstractions;

public interface IRoomStateResponseFactory
{
    public RoomStateResponse ToPlayerResponse(Guid playerId, InternalRoom room);
}
