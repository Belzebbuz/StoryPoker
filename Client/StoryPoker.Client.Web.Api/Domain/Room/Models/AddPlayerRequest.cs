using StoryPoker.Server.Abstractions.Room.Models;

namespace StoryPoker.Client.Web.Api.Domain.Room.Models;

public record AddPlayerRequest(string PlayerName)
{
    public RoomRequest.AddPlayer ToInternal(Guid userId)
        => new(userId, PlayerName);
};
