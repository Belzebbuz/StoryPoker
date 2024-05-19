using ErrorOr;
using StoryPoker.Server.Abstractions.Room.Models;
using StoryPoker.Server.Abstractions.Room.Models.Enums;

namespace StoryPoker.Server.Abstractions.Room.StateAbstractions;

public interface IRoomState
{
    ErrorOr<Success> SetIssuesNewOrder(Guid issueId, int newOrder);
    ErrorOr<Success> SetIssueOrderBy(IssueOrder order);
    ErrorOr<Success> AddNewPlayer(RoomRequest.AddPlayer request);
    ErrorOr<Success> RemovePlayer(Guid id);
    ErrorOr<Success> SetNewSpectator(Guid playerId);
    ErrorOr<Success> StartVote();
    ErrorOr<Success> StopVote();
    ErrorOr<Success> AddIssue(RoomRequest.AddIssue addIssueRequest);
    ErrorOr<Success> SetCurrentIssue(Guid issueId);
    ErrorOr<Success> SetStoryPoint(RoomRequest.SetStoryPoint request);
    ErrorOr<Success> RemoveIssue(Guid issueId);
}
