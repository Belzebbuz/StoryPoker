using StoryPoker.Server.Abstractions.Room.Models;

namespace StoryPoker.Client.Web.Api.Domain.Room.Models;

public record InitRoomStateRequest(string RoomName, string PlayerName)
{
    public RoomRequest.CreateRoom ToInternal(Guid playerId)
        => new(playerId, PlayerName, RoomName);
}
