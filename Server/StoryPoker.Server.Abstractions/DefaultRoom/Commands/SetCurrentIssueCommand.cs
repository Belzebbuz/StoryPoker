using ErrorOr;
using StoryPoker.Server.Abstractions.DefaultRoom.State;

namespace StoryPoker.Server.Abstractions.DefaultRoom.Commands;

[GenerateSerializer,Immutable]
public sealed record SetCurrentIssueCommand(
    [property: Id(0)] Guid IssueId) : DefaultRoomCommand
{
    public override ErrorOr<Success> Execute(IRoomState roomState)
    {
        return roomState.SetCurrentIssue(IssueId);
    }
}