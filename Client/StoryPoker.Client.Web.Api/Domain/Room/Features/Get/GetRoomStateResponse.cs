using StoryPoker.Server.Abstractions.Room.Models;

namespace StoryPoker.Client.Web.Api.Domain.Room.Features.Get;

public class GetRoomStateResponse
{

    public GetRoomStateResponse(Guid userId, RoomGrainState grainState)
    {
        Name = grainState.Name;
        VoteStarted = grainState.VoteStarted;
        VoteEnded = grainState.VoteEnded;
        VotingIssueId = grainState.VotingIssueId;
        Players = grainState.Players.ToDictionary(
            x => x.Key,
            x => (Player)x.Value);
        Issues = ToInternal(userId,grainState);
    }

    private static IDictionary<Guid,Issue> ToInternal(Guid userId, RoomGrainState grainState)
    {
        var result = new Dictionary<Guid, Issue>();
        foreach (var (_, issueState) in grainState.Issues)
        {
            var points = issueState.PlayerStoryPoints
                .ToDictionary(x => x.Key,
                    x =>
                    {
                        int? sp = x.Value;
                        return !grainState.VoteEnded ? null : sp;
                    });
            var issue = new Issue(issueState.Id, issueState.Title, issueState.StoryPoints, points);
            result[issue.Id] = issue;
        }
        return result;
    }

    public string Name { get; }
    public bool VoteStarted { get; }
    public bool VoteEnded { get; }
    public Guid? VotingIssueId { get; }
    public IDictionary<Guid, Player> Players { get; }
    public IDictionary<Guid, Issue> Issues { get; }

    public record Player(Guid Id, string Name, bool IsSpectator)
    {
        public static implicit operator Player(PlayerState state) =>
            new (state.Id, state.Name, state.IsSpectator);
    };

    public record Issue(Guid Id, string Title, int? StoryPoints, Dictionary<Guid, int?> PlayerStoryPoints);
}
