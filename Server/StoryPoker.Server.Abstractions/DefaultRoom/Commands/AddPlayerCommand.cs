using ErrorOr;
using StoryPoker.Server.Abstractions.DefaultRoom.Models;
using StoryPoker.Server.Abstractions.DefaultRoom.State;

namespace StoryPoker.Server.Abstractions.DefaultRoom.Commands;

[GenerateSerializer,Immutable]
public sealed record AddPlayerCommand(
    [property: Id(0)] Guid Id,
    [property: Id(1)] string Name) : DefaultRoomCommand
{
    public override ErrorOr<Success> Execute(IRoomState roomState)
    {
        var request = new DefaultRoomRequest.AddPlayer(Id, Name);
        return roomState.AddNewPlayer(request);
    }
}
