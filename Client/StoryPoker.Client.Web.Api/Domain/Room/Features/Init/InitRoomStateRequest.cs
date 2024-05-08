using StoryPoker.Server.Abstractions.Room.Models;

namespace StoryPoker.Client.Web.Api.Domain.Room.Features.Init;

public record InitRoomStateRequest(string RoomName, string PlayerName)
{
    public InitStateRequest ToInternal(Guid playerId)
        => new(playerId, PlayerName, RoomName);
}
