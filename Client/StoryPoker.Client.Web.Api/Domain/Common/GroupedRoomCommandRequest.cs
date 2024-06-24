using System.Text.Json.Serialization;
using StoryPoker.Server.Abstractions.DefaultRoom.Commands;
using StoryPoker.Server.Abstractions.DefaultRoom.Models.Enums;
using StoryPoker.Server.Abstractions.GroupedRoom.Commands;

namespace StoryPoker.Client.Web.Api.Domain.Common;

[JsonDerivedType(typeof(AddPlayerGroupedRoomRequest),nameof(AddPlayerGroupedRoomRequest))]
[JsonDerivedType(typeof(AddGroupsRoomRequest),nameof(AddGroupsRoomRequest))]
[JsonDerivedType(typeof(RenameGroupRoomRequest),nameof(RenameGroupRoomRequest))]
[JsonDerivedType(typeof(RemoveGroupRoomRequest),nameof(RemoveGroupRoomRequest))]
[JsonDerivedType(typeof(ChangePlayerGroupRoomRequest),nameof(ChangePlayerGroupRoomRequest))]
[JsonDerivedType(typeof(AddIssueGroupedRoomRequest),nameof(AddIssueGroupedRoomRequest))]
[JsonDerivedType(typeof(UpdateIssueGroupedRoomRequest),nameof(UpdateIssueGroupedRoomRequest))]
[JsonDerivedType(typeof(RemoveIssueGroupedRoomRequest),nameof(RemoveIssueGroupedRoomRequest))]
[JsonDerivedType(typeof(SetVotingIssueGroupedRoomRequest),nameof(SetVotingIssueGroupedRoomRequest))]
[JsonDerivedType(typeof(ChangeVotingStageGroupedRoomRequest),nameof(ChangeVotingStageGroupedRoomRequest))]
[JsonDerivedType(typeof(SetStoryPointGroupedRoomRequest),nameof(SetStoryPointGroupedRoomRequest))]
[JsonDerivedType(typeof(SetIssueOrderByGroupedRoomRequest),nameof(SetIssueOrderByGroupedRoomRequest))]
[JsonDerivedType(typeof(SetIssueOrderGroupedRoomRequest),nameof(SetIssueOrderGroupedRoomRequest))]
public abstract record GroupedRoomCommandRequest
{
    public abstract GroupedRoomCommand ToInternalCommand(Guid userId);
}

public sealed record AddPlayerGroupedRoomRequest(string PlayerName, string GroupName) : GroupedRoomCommandRequest
{
    public override GroupedRoomCommand ToInternalCommand(Guid userId)
        => new AddPlayerGroupedRoomCommand(userId, PlayerName, GroupName);
}
public sealed record AddGroupsRoomRequest(string[] GroupNames) : GroupedRoomCommandRequest
{
    public override GroupedRoomCommand ToInternalCommand(Guid userId)
        => new AddGroupsRoomCommand(GroupNames);
}
public sealed record RenameGroupRoomRequest(string OldName, string NewName) : GroupedRoomCommandRequest
{
    public override GroupedRoomCommand ToInternalCommand(Guid userId)
        => new RenameGroupRoomCommand(OldName, NewName);
}
public sealed record RemoveGroupRoomRequest(string GroupName) : GroupedRoomCommandRequest
{
    public override GroupedRoomCommand ToInternalCommand(Guid userId)
        => new RemoveGroupRoomCommand(GroupName);
}
public sealed record ChangePlayerGroupRoomRequest(string GroupName) : GroupedRoomCommandRequest
{
    public override GroupedRoomCommand ToInternalCommand(Guid userId)
        => new ChangePlayerGroupRoomCommand(userId,GroupName);
}
public sealed record AddIssueGroupedRoomRequest(string Title, string[] GroupNames) : GroupedRoomCommandRequest
{
    public override GroupedRoomCommand ToInternalCommand(Guid userId)
        => new AddIssueGroupedRoomCommand(Title,GroupNames);
}
public sealed record UpdateIssueGroupedRoomRequest(Guid IssueId, string Title, string[] GroupNames) : GroupedRoomCommandRequest
{
    public override GroupedRoomCommand ToInternalCommand(Guid userId)
        => new UpdateIssueGroupedRoomCommand(IssueId,Title,GroupNames);
}
public sealed record RemoveIssueGroupedRoomRequest(Guid IssueId) : GroupedRoomCommandRequest
{
    public override GroupedRoomCommand ToInternalCommand(Guid userId)
        => new RemoveIssueGroupedRoomCommand(IssueId);
}
public sealed record SetVotingIssueGroupedRoomRequest(Guid IssueId) : GroupedRoomCommandRequest
{
    public override GroupedRoomCommand ToInternalCommand(Guid userId)
        => new SetVotingIssueGroupedRoomCommand(IssueId);
}
public sealed record ChangeVotingStageGroupedRoomRequest(VoteStageChangeType VotingStage) : GroupedRoomCommandRequest
{
    public override GroupedRoomCommand ToInternalCommand(Guid userId)
        => new ChangeVotingStageGroupedRoomCommand(VotingStage);
}

public sealed record SetStoryPointGroupedRoomRequest(int StoryPoint) : GroupedRoomCommandRequest
{
    public override GroupedRoomCommand ToInternalCommand(Guid userId)
        => new SetStoryPointGroupedRoomCommand(userId, StoryPoint);
}
public sealed record SetIssueOrderByGroupedRoomRequest(IssueOrder OrderBy) : GroupedRoomCommandRequest
{
    public override GroupedRoomCommand ToInternalCommand(Guid userId)
        => new SetIssuesOrderByGroupedRoomCommand(OrderBy);
}
public sealed record SetIssueOrderGroupedRoomRequest(Guid IssueId, int NewOrder) : GroupedRoomCommandRequest
{
    public override GroupedRoomCommand ToInternalCommand(Guid userId)
        => new SetIssueOrderGroupedRoomCommand(IssueId, NewOrder);
}
