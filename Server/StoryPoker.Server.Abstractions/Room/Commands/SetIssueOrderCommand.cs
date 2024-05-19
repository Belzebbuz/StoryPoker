using ErrorOr;
using StoryPoker.Server.Abstractions.Room.StateAbstractions;

namespace StoryPoker.Server.Abstractions.Room.Commands;

[GenerateSerializer,Immutable]
public sealed record SetIssueOrderCommand(
    [property: Id(0)] Guid IssueId,
    [property: Id(1)] int NewOrder) : RoomCommand
{
    public override ErrorOr<Success> Execute(IRoomState roomState)
    {
        return roomState.SetIssuesNewOrder(IssueId, NewOrder);
    }
}