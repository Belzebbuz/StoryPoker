using ErrorOr;
using StoryPoker.Server.Abstractions.Room.StateAbstractions;

namespace StoryPoker.Server.Abstractions.Room.Commands;

[GenerateSerializer,Immutable]
public sealed record SetCurrentIssueCommand(
    [property: Id(0)] Guid IssueId) : RoomCommand
{
    public override ErrorOr<Success> Execute(IRoomState roomState)
    {
        return roomState.SetCurrentIssue(IssueId);
    }
}