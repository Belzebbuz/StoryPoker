using ErrorOr;
using StoryPoker.Server.Abstractions.Room.Models;
using StoryPoker.Server.Abstractions.Room.StateAbstractions;

namespace StoryPoker.Server.Abstractions.Room.Commands;

[GenerateSerializer,Immutable]
public sealed record AddPlayerCommand(
    [property: Id(0)] Guid Id,
    [property: Id(1)] string Name) : RoomCommand
{
    public override ErrorOr<Success> Execute(IRoomState roomState)
    {
        var request = new RoomRequest.AddPlayer(Id, Name);
        return roomState.AddNewPlayer(request);
    }
}