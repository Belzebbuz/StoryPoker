using ErrorOr;
using StoryPoker.Server.Abstractions.Room.Models.Enums;
using StoryPoker.Server.Abstractions.Room.StateAbstractions;

namespace StoryPoker.Server.Abstractions.Room.Commands;

[GenerateSerializer,Immutable]
public sealed record SetIssueListOrderCommand(
    [property: Id(0)] IssueOrder OrderBy) : RoomCommand
{
    public override ErrorOr<Success> Execute(IRoomState roomState)
    {
        return roomState.SetIssueOrderBy(OrderBy);
    }
}