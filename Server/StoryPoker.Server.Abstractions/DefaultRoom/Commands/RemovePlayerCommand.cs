using ErrorOr;
using StoryPoker.Server.Abstractions.DefaultRoom.State;

namespace StoryPoker.Server.Abstractions.DefaultRoom.Commands;

[GenerateSerializer,Immutable]
public sealed record RemovePlayerCommand(
    [property: Id(0)] Guid Id) : DefaultRoomCommand
{
    public override ErrorOr<Success> Execute(IRoomState roomState)
    {
        return roomState.RemovePlayer(Id);
    }
}