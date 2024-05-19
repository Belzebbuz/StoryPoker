using ErrorOr;
using StoryPoker.Server.Abstractions.Room.Models;
using StoryPoker.Server.Abstractions.Room.StateAbstractions;

namespace StoryPoker.Server.Abstractions.Room.Commands;

[GenerateSerializer,Immutable]
public sealed record AddIssueCommand(
    [property: Id(0)] string Title) : RoomCommand
{
    public override ErrorOr<Success> Execute(IRoomState roomState)
    {
        var request = new RoomRequest.AddIssue(Title);
        return roomState.AddIssue(request);
    }
}