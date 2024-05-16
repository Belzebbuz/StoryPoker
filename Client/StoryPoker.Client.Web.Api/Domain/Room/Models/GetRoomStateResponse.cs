using StoryPoker.Server.Abstractions.Room.Models;

namespace StoryPoker.Client.Web.Api.Domain.Room.Models;

public class GetRoomStateResponse
{
    public GetRoomStateResponse(Guid userId, RoomGrainState grainState)
    {
        PlayerId = userId;
        IsPlayerAdded = grainState.Players.ContainsKey(userId);
        Name = grainState.Name;
        VotingIssue = GetVotingIssue(grainState);
        IsSpectator = grainState.Players.GetValueOrDefault(userId)?.IsSpectator ?? false;
        Players = GetPlayers(userId, grainState);
        Issues = GetIssues(grainState);
        IssueOrder = grainState.IssueOrderBy;
    }

    private static IEnumerable<Issue> GetIssues(RoomGrainState state)
    {
        var issues = state.Issues.Values;
        issues = state.IssueOrderBy switch
        {
            IssueOrder.Asc => issues.OrderBy(x => x.Order),
            IssueOrder.Desc => issues.OrderByDescending(x => x.Order),
            _ => issues.OrderBy(x => x.Order)
        };

        return issues.Select(issueState =>
            new Issue(issueState.Id, issueState.Title, issueState.Stage, issueState.StoryPoints, issueState.Order));
    }

    private static IEnumerable<Player> GetPlayers(Guid userId, RoomGrainState grainState) =>
        grainState.Players.Values
            .OrderBy(x => x.Order)
            .Select(
                playerState =>
                {
                    var playerVoted =
                        grainState.VotingIssueId.HasValue &&
                        grainState.Issues[grainState.VotingIssueId.Value].PlayerStoryPoints
                            .ContainsKey(playerState.Id);
                    var canShowValue =
                        (grainState.VotingIssueId.HasValue &&
                         grainState.Issues[grainState.VotingIssueId.Value].Stage != VotingStage.Voting)
                        || userId == playerState.Id;
                    int? playerVotePoint = playerVoted && canShowValue
                        ? grainState.Issues[grainState.VotingIssueId!.Value].PlayerStoryPoints[playerState.Id]
                        : null;
                    return new Player(playerState.Id, playerState.Name, playerState.IsSpectator,
                        new PlayerIssueStoryPoint(playerVoted, playerVotePoint));
                });


    private static Issue? GetVotingIssue(RoomGrainState grainState)
    {
        if (!grainState.VotingIssueId.HasValue)
            return null;
        var issue = grainState.Issues[grainState.VotingIssueId.Value];
        return new Issue(issue.Id, issue.Title, issue.Stage, issue.StoryPoints, issue.Order);
    }

    public Guid PlayerId { get; }
    public bool IsPlayerAdded { get; }
    public bool IsSpectator { get; }
    public string Name { get; }
    public Issue? VotingIssue { get; }
    public IEnumerable<Player> Players { get; }
    public IEnumerable<Issue> Issues { get; }
    public IssueOrder IssueOrder { get; }

    public record Player(Guid Id, string Name, bool IsSpectator, PlayerIssueStoryPoint CurrentVotingPoint);

    public record PlayerIssueStoryPoint(bool Voted, int? Value);

    public record Issue(Guid Id, string Title, VotingStage Stage, int? StoryPoints, int Order);
}
