using System.Text.Json.Serialization;
using StoryPoker.Server.Abstractions.DefaultRoom.Commands;
using StoryPoker.Server.Abstractions.DefaultRoom.Models.Enums;

namespace StoryPoker.Client.Web.Api.Domain.Common;

[JsonDerivedType(typeof(AddPlayerDefaultRoomRequest),nameof(AddPlayerDefaultRoomRequest))]
[JsonDerivedType(typeof(SelectSpectatorDefaultRoomRequest),nameof(SelectSpectatorDefaultRoomRequest))]
[JsonDerivedType(typeof(RemovePlayerDefaultRoomRequest),nameof(RemovePlayerDefaultRoomRequest))]
[JsonDerivedType(typeof(ChangeVoteStageDefaultRoomRequest),nameof(ChangeVoteStageDefaultRoomRequest))]
[JsonDerivedType(typeof(AddIssueDefaultRoomRequest),nameof(AddIssueDefaultRoomRequest))]
[JsonDerivedType(typeof(SetIssueListOrderDefaultRoomRequest),nameof(SetIssueListOrderDefaultRoomRequest))]
[JsonDerivedType(typeof(SetIssueOrderDefaultRoomRequest),nameof(SetIssueOrderDefaultRoomRequest))]
[JsonDerivedType(typeof(RemoveIssueDefaultRoomRequest),nameof(RemoveIssueDefaultRoomRequest))]
[JsonDerivedType(typeof(SetCurrentIssueDefaultRoomRequest),nameof(SetCurrentIssueDefaultRoomRequest))]
[JsonDerivedType(typeof(SetStoryPointDefaultRoomRequest),nameof(SetStoryPointDefaultRoomRequest))]
[JsonDerivedType(typeof(UpdateSettingsDefaultRoomRequest),nameof(UpdateSettingsDefaultRoomRequest))]
public abstract record DefaultRoomCommandRequest
{
    public abstract DefaultRoomCommand ToInternalCommand(Guid userId);
}
public sealed record UpdateSettingsDefaultRoomRequest(bool SpectatorCanVote, bool SkipBorderValues) : DefaultRoomCommandRequest
{
    public override DefaultRoomCommand ToInternalCommand(Guid userId)
        => new UpdateSettingsCommand(SpectatorCanVote, SkipBorderValues);
}
public sealed record AddPlayerDefaultRoomRequest(string PlayerName) : DefaultRoomCommandRequest, IPlayerNameRequest
{
    public override DefaultRoomCommand ToInternalCommand(Guid userId)
            => new AddPlayerCommand(userId, PlayerName);
}

public sealed record SelectSpectatorDefaultRoomRequest(Guid PlayerId) : DefaultRoomCommandRequest
{
    public override DefaultRoomCommand ToInternalCommand(Guid userId)
        => new SetNewSpectatorCommand(PlayerId);
}
public sealed record RemovePlayerDefaultRoomRequest(Guid PlayerId) : DefaultRoomCommandRequest
{
    public override DefaultRoomCommand ToInternalCommand(Guid userId)
        => new RemovePlayerCommand(PlayerId);
}
public sealed record ChangeVoteStageDefaultRoomRequest(VoteStageChangeType Stage) : DefaultRoomCommandRequest
{
    public override DefaultRoomCommand ToInternalCommand(Guid userId)
        => new ChangeVotingStageCommand(Stage);
}

public sealed record AddIssueDefaultRoomRequest(string Title) : DefaultRoomCommandRequest
{
    public override DefaultRoomCommand ToInternalCommand(Guid userId)
        => new AddIssueCommand(Title);
}
public sealed record SetIssueListOrderDefaultRoomRequest(IssueOrder Order) : DefaultRoomCommandRequest
{
    public override DefaultRoomCommand ToInternalCommand(Guid userId)
        => new SetIssueListOrderCommand(Order);
}
public sealed record SetIssueOrderDefaultRoomRequest(Guid IssueId, int NewOrder) : DefaultRoomCommandRequest
{
    public override DefaultRoomCommand ToInternalCommand(Guid userId)
        => new SetIssueOrderCommand(IssueId, NewOrder);
}
public sealed record RemoveIssueDefaultRoomRequest(Guid IssueId) : DefaultRoomCommandRequest
{
    public override DefaultRoomCommand ToInternalCommand(Guid userId)
        => new RemoveIssueCommand(IssueId);
}
public sealed record SetCurrentIssueDefaultRoomRequest(Guid IssueId) : DefaultRoomCommandRequest
{
    public override DefaultRoomCommand ToInternalCommand(Guid userId)
        => new SetCurrentIssueCommand(IssueId);
}
public sealed record SetStoryPointDefaultRoomRequest(int StoryPoint) : DefaultRoomCommandRequest
{
    public override DefaultRoomCommand ToInternalCommand(Guid userId)
        => new SetStoryPointCommand(userId, StoryPoint);
}
