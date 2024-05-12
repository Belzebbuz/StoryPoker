using StoryPoker.Server.Abstractions.Room.Models;

namespace StoryPoker.Client.Web.Api.Domain.Room.Features.AddPlayer;

public record AddPlayerRequest(string PlayerName)
{
    public RoomRequest.AddPlayer ToInternal(Guid userId)
        => new(userId, PlayerName);
};
