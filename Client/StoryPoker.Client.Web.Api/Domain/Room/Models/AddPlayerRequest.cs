using StoryPoker.Server.Abstractions.Room.Commands;

namespace StoryPoker.Client.Web.Api.Domain.Room.Models;

public record AddPlayerRequest(string PlayerName)
{
    public AddPlayerCommand ToCommand(Guid userId)
        => new(userId, PlayerName);
};
