using ErrorOr;
using StoryPoker.Server.Abstractions.DefaultRoom.Models;
using StoryPoker.Server.Abstractions.DefaultRoom.Models.Enums;

namespace StoryPoker.Server.Abstractions.DefaultRoom.State;

public interface IRoomState
{
    ErrorOr<Success> SetIssuesNewOrder(Guid issueId, int newOrder);
    ErrorOr<Success> SetIssueOrderBy(IssueOrder order);
    ErrorOr<Success> AddNewPlayer(DefaultRoomRequest.AddPlayer request);
    ErrorOr<Success> RemovePlayer(Guid id);
    ErrorOr<Success> SetNewSpectator(Guid playerId);
    ErrorOr<Success> StartVote();
    ErrorOr<Success> StopVote();
    ErrorOr<Success> SetEndingTimerVote();
    ErrorOr<Success> AddIssue(DefaultRoomRequest.AddIssue addIssueRequest);
    ErrorOr<Success> SetCurrentIssue(Guid issueId);
    ErrorOr<Success> SetStoryPoint(DefaultRoomRequest.SetStoryPoint request);
    ErrorOr<Success> RemoveIssue(Guid issueId);
}
