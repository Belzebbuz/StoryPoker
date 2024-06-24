using ErrorOr;
using StoryPoker.Server.Abstractions.DefaultRoom.Commands;
using StoryPoker.Server.Abstractions.DefaultRoom.Models.Enums;

namespace StoryPoker.Server.Abstractions.GroupedRoom.State;

public interface IGroupedRoomState
{
    ErrorOr<Success> AddPlayer(Guid userId, string playerName, string groupName);
    ErrorOr<Success> AddGroupRange(ICollection<string> groupNames);
    ErrorOr<Success> RemovePlayer(Guid userId);
    ErrorOr<Success> RenameGroup(string oldName, string newName);
    ErrorOr<Success> RemoveGroup(string groupName);
    ErrorOr<Success> ChangePlayerGroup(Guid playerId, string groupName);
    ErrorOr<Success> AddIssue(string title, string[] groupNames);
    ErrorOr<Success> UpdateIssue(Guid issueId, string title, string[] groupNames);
    ErrorOr<Success> RemoveIssue(Guid issueId);
    ErrorOr<Success> SetVotingIssue(Guid issueId);
    ErrorOr<Success> ChangeVotingStage(VoteStageChangeType votingStage);
    ErrorOr<Success> SetStoryPoint(Guid playerId, int storyPoint);
    ErrorOr<Success> SetIssuesOrderBy(IssueOrder orderBy);
    ErrorOr<Success> SetIssueOrder(Guid issueId, int newOrder);
}
