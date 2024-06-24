using ErrorOr;
using StoryPoker.Server.Abstractions.DefaultRoom.State;

namespace StoryPoker.Server.Abstractions.DefaultRoom.Commands;

[GenerateSerializer,Immutable]
public sealed record SetNewSpectatorCommand(
    [property: Id(0)] Guid PlayerId) : DefaultRoomCommand
{
    public override ErrorOr<Success> Execute(IRoomState roomState)
    {
        return roomState.SetNewSpectator(PlayerId);
    }
}