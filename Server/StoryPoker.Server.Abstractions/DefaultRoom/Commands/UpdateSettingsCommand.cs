using ErrorOr;
using StoryPoker.Server.Abstractions.DefaultRoom.State;

namespace StoryPoker.Server.Abstractions.DefaultRoom.Commands;

[GenerateSerializer,Immutable]
public sealed record UpdateSettingsCommand(
    [property: Id(0)] bool SpectatorCanVote,
    [property: Id(1)] bool SkipBorderValues) : DefaultRoomCommand
{
    public override ErrorOr<Success> Execute(IRoomState roomState)
    {
        return roomState.SetSpectatorVote(SpectatorCanVote, SkipBorderValues);
    }
}
