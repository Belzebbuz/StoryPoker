using ErrorOr;
using StoryPoker.Server.Abstractions.Room.Models;
using StoryPoker.Server.Abstractions.Room.StateAbstractions;

namespace StoryPoker.Server.Abstractions.Room.Commands;

[GenerateSerializer,Immutable]
public sealed record SetPlayerIssueStoryPointCommand(
    [property: Id(0)] Guid PlayerId,
    [property: Id(1)]int StoryPoints) : RoomCommand
{
    public override ErrorOr<Success> Execute(IRoomState roomState)
    {
        var request = new RoomRequest.SetStoryPoint(PlayerId, StoryPoints);
        return roomState.SetStoryPoint(request);
    }
}