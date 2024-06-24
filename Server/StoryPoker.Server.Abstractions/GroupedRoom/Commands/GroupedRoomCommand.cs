using ErrorOr;
using StoryPoker.Server.Abstractions.Common;
using StoryPoker.Server.Abstractions.DefaultRoom.Commands;
using StoryPoker.Server.Abstractions.DefaultRoom.Models.Enums;
using StoryPoker.Server.Abstractions.GroupedRoom.State;

namespace StoryPoker.Server.Abstractions.GroupedRoom.Commands;

[GenerateSerializer, Immutable]
public abstract record GroupedRoomCommand : IRoomCommand
{
    public abstract ErrorOr<Success> Execute(IGroupedRoomState roomState);
}

[GenerateSerializer, Immutable]
public record AddPlayerGroupedRoomCommand(
    [property: Id(0)] Guid UserId,
    [property: Id(1)] string PlayerName,
    [property: Id(2)] string GroupName) : GroupedRoomCommand
{
    public override ErrorOr<Success> Execute(IGroupedRoomState roomState)
    {
        return roomState.AddPlayer(UserId, PlayerName, GroupName);
    }
}
[GenerateSerializer, Immutable]
public record RemovePlayerGroupedRoomCommand(
    [property: Id(0)] Guid UserId) : GroupedRoomCommand
{
    public override ErrorOr<Success> Execute(IGroupedRoomState roomState)
    {
        return roomState.RemovePlayer(UserId);
    }
}
[GenerateSerializer, Immutable]
public record AddGroupsRoomCommand(
    [property: Id(1)] string[] GroupNames) : GroupedRoomCommand
{
    public override ErrorOr<Success> Execute(IGroupedRoomState roomState)
    {
        return roomState.AddGroupRange(GroupNames);
    }
}
[GenerateSerializer, Immutable]
public record RenameGroupRoomCommand(
    [property: Id(0)] string OldName,
    [property: Id(1)] string NewName) : GroupedRoomCommand
{
    public override ErrorOr<Success> Execute(IGroupedRoomState roomState)
    {
        return roomState.RenameGroup(OldName, NewName);
    }
}

[GenerateSerializer, Immutable]
public record RemoveGroupRoomCommand(
    [property: Id(0)] string GroupName) : GroupedRoomCommand
{
    public override ErrorOr<Success> Execute(IGroupedRoomState roomState)
    {
        return roomState.RemoveGroup(GroupName);
    }
}
[GenerateSerializer, Immutable]
public record ChangePlayerGroupRoomCommand(
    [property: Id(0)] Guid PlayerId,
    [property: Id(1)] string GroupName) : GroupedRoomCommand
{
    public override ErrorOr<Success> Execute(IGroupedRoomState roomState)
    {
        return roomState.ChangePlayerGroup(PlayerId, GroupName);
    }
}

[GenerateSerializer, Immutable]
public record AddIssueGroupedRoomCommand(
    [property: Id(0)] string Title,
    [property: Id(1)] string[] GroupNames) : GroupedRoomCommand
{
    public override ErrorOr<Success> Execute(IGroupedRoomState roomState)
    {
        return roomState.AddIssue(Title, GroupNames);
    }
}
[GenerateSerializer, Immutable]
public record UpdateIssueGroupedRoomCommand(
    [property:Id(0)] Guid IssueId,
    [property: Id(1)] string Title,
    [property: Id(2)] string[] GroupNames) : GroupedRoomCommand
{
    public override ErrorOr<Success> Execute(IGroupedRoomState roomState)
    {
        return roomState.UpdateIssue(IssueId,Title, GroupNames);
    }
}
[GenerateSerializer, Immutable]
public record RemoveIssueGroupedRoomCommand(
    [property:Id(0)] Guid IssueId) : GroupedRoomCommand
{
    public override ErrorOr<Success> Execute(IGroupedRoomState roomState)
    {
        return roomState.RemoveIssue(IssueId);
    }
}

[GenerateSerializer, Immutable]
public record SetVotingIssueGroupedRoomCommand(
    [property:Id(0)] Guid IssueId) : GroupedRoomCommand
{
    public override ErrorOr<Success> Execute(IGroupedRoomState roomState)
    {
        return roomState.SetVotingIssue(IssueId);
    }
}

[GenerateSerializer, Immutable]
public record ChangeVotingStageGroupedRoomCommand(
    [property:Id(0)] VoteStageChangeType VotingStage) : GroupedRoomCommand
{
    public override ErrorOr<Success> Execute(IGroupedRoomState roomState)
    {
        return roomState.ChangeVotingStage(VotingStage);
    }
}
[GenerateSerializer, Immutable]
public record SetStoryPointGroupedRoomCommand(
    [property:Id(0)] Guid PlayerId,
    [property:Id(1)] int StoryPoint) : GroupedRoomCommand
{
    public override ErrorOr<Success> Execute(IGroupedRoomState roomState)
    {
        return roomState.SetStoryPoint(PlayerId,StoryPoint);
    }
}

[GenerateSerializer, Immutable]
public record SetIssuesOrderByGroupedRoomCommand(
    [property:Id(0)] IssueOrder OrderBy) : GroupedRoomCommand
{
    public override ErrorOr<Success> Execute(IGroupedRoomState roomState)
    {
        return roomState.SetIssuesOrderBy(OrderBy);
    }
}

[GenerateSerializer, Immutable]
public record SetIssueOrderGroupedRoomCommand(
    [property:Id(0)] Guid IssueId,
    [property:Id(1)] int NewOrder) : GroupedRoomCommand
{
    public override ErrorOr<Success> Execute(IGroupedRoomState roomState)
    {
        return roomState.SetIssueOrder(IssueId, NewOrder);
    }
}
