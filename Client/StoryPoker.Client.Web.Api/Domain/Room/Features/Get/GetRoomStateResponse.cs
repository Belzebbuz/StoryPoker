using StoryPoker.Server.Abstractions.Room.Models;

namespace StoryPoker.Client.Web.Api.Domain.Room.Features.Get;

public class GetRoomStateResponse
{
    public GetRoomStateResponse(Guid userId, RoomGrainState grainState)
    {
        PlayerId = userId;
        IsPlayerAdded = grainState.Players.ContainsKey(userId);
        Name = grainState.Name;
        VotingIssue = GetVotingIssue(grainState);
        IsSpectator = grainState.Players.GetValueOrDefault(userId)?.IsSpectator ?? false;
        Players = grainState.Players.Values
            .OrderBy(x => x.Order)
            .Select(
                x =>
                {
                    var playerVoted =
                        grainState.VotingIssueId.HasValue &&
                        grainState.Issues[grainState.VotingIssueId.Value].PlayerStoryPoints
                            .ContainsKey(x.Id);
                    var canShowValue =
                        grainState.VotingIssueId.HasValue &&
                        grainState.Issues[grainState.VotingIssueId.Value].Stage != VotingStage.Voting;
                    int? playerVotePoint = playerVoted && canShowValue
                        ? grainState.Issues[grainState.VotingIssueId!.Value].PlayerStoryPoints[x.Id]
                        : null;
                    return new Player(x.Id, x.Name, x.IsSpectator,
                        new PlayerIssueStoryPoint(playerVoted, playerVotePoint));
                }).ToArray().AsReadOnly();
        Issues = grainState.Issues
            .Values
            .OrderByDescending(x => x.Order)
            .Select(
                issueState =>
                    new Issue(issueState.Id, issueState.Title, VotingStage.NotStarted, issueState.StoryPoints))
            .ToArray().AsReadOnly();
    }

    private static Issue? GetVotingIssue(RoomGrainState grainState)
    {
        if (!grainState.VotingIssueId.HasValue)
            return null;
        var issue = grainState.Issues[grainState.VotingIssueId.Value];
        return new Issue(issue.Id, issue.Title, issue.Stage, issue.StoryPoints);
    }

    public Guid PlayerId { get; }
    public bool IsPlayerAdded { get; }
    public bool IsSpectator { get; }
    public string Name { get; }
    public Issue? VotingIssue { get; }
    public IReadOnlyCollection<Player> Players { get; }
    public IReadOnlyCollection<Issue> Issues { get; }

    public record Player(Guid Id, string Name, bool IsSpectator, PlayerIssueStoryPoint CurrentVotingPoint);

    public record PlayerIssueStoryPoint(bool Voted, int? Value);

    public record Issue(Guid Id, string Title, VotingStage Stage, int? StoryPoints);
}
