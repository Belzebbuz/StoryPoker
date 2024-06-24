using ErrorOr;
using StoryPoker.Server.Abstractions.DefaultRoom.Models;
using StoryPoker.Server.Abstractions.DefaultRoom.State;

namespace StoryPoker.Server.Abstractions.DefaultRoom.Commands;

[GenerateSerializer,Immutable]
public sealed record AddIssueCommand(
    [property: Id(0)] string Title) : DefaultRoomCommand
{
    public override ErrorOr<Success> Execute(IRoomState roomState)
    {
        var request = new DefaultRoomRequest.AddIssue(Title);
        return roomState.AddIssue(request);
    }
}