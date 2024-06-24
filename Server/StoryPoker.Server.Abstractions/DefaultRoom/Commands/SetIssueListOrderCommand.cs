using ErrorOr;
using StoryPoker.Server.Abstractions.DefaultRoom.Models.Enums;
using StoryPoker.Server.Abstractions.DefaultRoom.State;

namespace StoryPoker.Server.Abstractions.DefaultRoom.Commands;

[GenerateSerializer,Immutable]
public sealed record SetIssueListOrderCommand(
    [property: Id(0)] IssueOrder OrderBy) : DefaultRoomCommand
{
    public override ErrorOr<Success> Execute(IRoomState roomState)
    {
        return roomState.SetIssueOrderBy(OrderBy);
    }
}