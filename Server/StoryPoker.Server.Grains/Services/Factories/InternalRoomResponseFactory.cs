using StoryPoker.Server.Abstractions.Room.Models;
using StoryPoker.Server.Abstractions.Room.Models.Enums;
using StoryPoker.Server.Grains.Abstractions;
using StoryPoker.Server.Grains.RoomGrains.Models;

namespace StoryPoker.Server.Grains.Services.Factories;

public class InternalRoomResponseFactory : IRoomStateResponseFactory
{
    public RoomStateResponse ToPlayerResponse(Guid playerId, InternalRoom room)
    {
        return new RoomStateResponse()
        {
            PlayerId = playerId,
            IsPlayerAdded = room.Players.ContainsKey(playerId),
            Name = room.Name,
            VotingIssue = GetVotingIssue(room),
            IsSpectator = room.Players.TryGetValue(playerId, out var player) && player.IsSpectator,
            Players = GetPlayers(playerId,room),
            Issues = GetIssues(room),
            IssueOrder = room.IssueOrderBy
        };
    }

    private static ICollection<IssueStateResponse> GetIssues(InternalRoom state)
    {
        var issues = state.Issues.Values.AsEnumerable();
        issues = state.IssueOrderBy switch
        {
            IssueOrder.Asc => issues.OrderBy(x => x.Order),
            IssueOrder.Desc => issues.OrderByDescending(x => x.Order),
            _ => issues.OrderBy(x => x.Order)
        };

        return issues.Select(issueState =>
            new IssueStateResponse()
            {
                Id = issueState.Id,
                Order = issueState.Order,
                Stage = issueState.Stage,
                Title = issueState.Title,
                StoryPoints = issueState.StoryPoints,
                FibonacciStoryPoints = issueState.FibonacciStoryPoints
            }).ToArray();
    }

    private static ICollection<PlayerStateResponse> GetPlayers(Guid userId, InternalRoom state) =>
        state.Players.Values
            .OrderBy(x => x.Order)
            .Select(
                playerState =>
                {
                    var playerVoted =
                        state.VotingIssueId.HasValue &&
                        state.Issues[state.VotingIssueId.Value].PlayerStoryPoints
                            .ContainsKey(playerState.Id);
                    var canShowValue =
                        (state.VotingIssueId.HasValue &&
                         state.Issues[state.VotingIssueId.Value].Stage != VotingStage.Voting)
                        || userId == playerState.Id;
                    int? playerVotePoint = playerVoted && canShowValue
                        ? state.Issues[state.VotingIssueId!.Value].PlayerStoryPoints[playerState.Id]
                        : null;
                    return new PlayerStateResponse()
                    {
                        Id = playerState.Id,
                        Name = playerState.Name,
                        IsSpectator = playerState.IsSpectator,
                        CurrentVotingPoint = new PlayerIssueStoryPoint()
                        {
                            Value = playerVotePoint,
                            Voted = playerVoted
                        }
                    };
                }).ToArray();


    private static IssueStateResponse? GetVotingIssue(InternalRoom state)
    {
        if (!state.VotingIssueId.HasValue)
            return null;
        var issueState = state.Issues[state.VotingIssueId.Value];
        return  new IssueStateResponse()
        {
            Id = issueState.Id,
            Order = issueState.Order,
            Stage = issueState.Stage,
            Title = issueState.Title,
            StoryPoints = issueState.StoryPoints,
            FibonacciStoryPoints = issueState.FibonacciStoryPoints
        };
    }
}
