using ErrorOr;
using StoryPoker.Server.Abstractions.Room.StateAbstractions;

namespace StoryPoker.Server.Abstractions.Room.Commands;

[GenerateSerializer,Immutable]
public sealed record RemovePlayerCommand(
    [property: Id(0)] Guid Id) : RoomCommand
{
    public override ErrorOr<Success> Execute(IRoomState roomState)
    {
        return roomState.RemovePlayer(Id);
    }
}