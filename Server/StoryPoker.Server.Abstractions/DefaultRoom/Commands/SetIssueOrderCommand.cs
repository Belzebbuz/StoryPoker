using ErrorOr;
using StoryPoker.Server.Abstractions.DefaultRoom.State;

namespace StoryPoker.Server.Abstractions.DefaultRoom.Commands;

[GenerateSerializer,Immutable]
public sealed record SetIssueOrderCommand(
    [property: Id(0)] Guid IssueId,
    [property: Id(1)] int NewOrder) : DefaultRoomCommand
{
    public override ErrorOr<Success> Execute(IRoomState roomState)
    {
        return roomState.SetIssuesNewOrder(IssueId, NewOrder);
    }
}