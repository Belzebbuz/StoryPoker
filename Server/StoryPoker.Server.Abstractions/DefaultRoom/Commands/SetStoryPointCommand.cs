using ErrorOr;
using StoryPoker.Server.Abstractions.DefaultRoom.Models;
using StoryPoker.Server.Abstractions.DefaultRoom.State;

namespace StoryPoker.Server.Abstractions.DefaultRoom.Commands;

[GenerateSerializer,Immutable]
public sealed record SetStoryPointCommand(
    [property: Id(0)] Guid PlayerId,
    [property: Id(1)]int StoryPoints) : DefaultRoomCommand
{
    public override ErrorOr<Success> Execute(IRoomState roomState)
    {
        var request = new DefaultRoomRequest.SetStoryPoint(PlayerId, StoryPoints);
        return roomState.SetStoryPoint(request);
    }
}