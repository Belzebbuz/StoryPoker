using ErrorOr;
using Newtonsoft.Json;

namespace StoryPoker.Server.Abstractions.Room.Models;

[GenerateSerializer, Immutable]
public record RoomGrainState
{
    [Id(4)] public required string Name { get; init; }
    public IReadOnlyDictionary<Guid, PlayerState> Players => _players;
    [Id(0)] private readonly Dictionary<Guid, PlayerState> _players = new();
    public IReadOnlyDictionary<Guid, IssueState> Issues => _issues;
    [Id(1)] private readonly Dictionary<Guid, IssueState> _issues = new();
    [Id(2), JsonProperty] public Guid? VotingIssueId { get; private set; }
    [Id(3), JsonProperty] public bool Instantiated { get; private set; }
    [Id(5), JsonProperty] public bool VoteStarted { get; private set; }
    [Id(6), JsonProperty] public bool VoteEnded { get; private set; }

    public static RoomGrainState Init(InitStateRequest request)
    {
        var playerState = new PlayerState() { Name = request.PlayerName, IsSpectator = true, Id = request.PlayerId };
        var state = new RoomGrainState()
        {
            Name = request.RoomName,
            Instantiated = true
        };
        state.AddPlayer(playerState);
        return state;
    }

    public ErrorOr<Success> AddPlayer(AddPlayerRequest request)
    {
        var playerExist =_players.ContainsKey(request.Id);
        if (playerExist)
            return Result.Success;

        var player = new PlayerState()
        {
            Id = request.Id,
            Name = request.Name,
            IsSpectator = _players.Count == 0
        };
        AddPlayer(player);
        return Result.Success;
    }

    private void AddPlayer(PlayerState player)
    {
        _players.Add(player.Id, player);
    }

    public ErrorOr<Success> StartVote()
    {
        if (!VotingIssueId.HasValue)
            return Error.Failure(description: "Не выбран объект голосвания");
        VoteStarted = true;
        VoteEnded = false;
        return Result.Success;
    }

    public ErrorOr<Success> StopVote()
    {
        if (!VoteStarted)
            return Error.Failure(description: "Голосование не начато");
        if (!VotingIssueId.HasValue)
            return Error.Failure(description: "Не выбран объект голосвания");
        VoteEnded = true;
        VoteStarted = false;
        var issue = Issues[VotingIssueId.Value];
        issue.StoryPoints = (int)issue.PlayerStoryPoints.Average(x => x.Value);
        return Result.Success;
    }

    public void AddIssue(AddIssueRequest addIssueRequest)
    {
        var issue = new IssueState() { Id = Guid.NewGuid(), Title = addIssueRequest.Title, };
        _issues.Add(issue.Id, issue);
    }

    public ErrorOr<Success> SetCurrentIssue(Guid issueId)
    {
        var issue = _issues.GetValueOrDefault(issueId);
        if (issue is null)
            return Error.Failure("Не найдена задача");
        if (VoteStarted && issue.Id != VotingIssueId)
            return Error.Failure("Голосование по текущему объекту еще не закончилось");
        VotingIssueId = issue.Id;
        VoteEnded = false;
        VoteStarted = false;
        return Result.Success;
    }

    public ErrorOr<Success> SetStoryPoint(SetStoryPointRequest request)
    {
        var playerExist = _players.ContainsKey(request.PlayerId);
        if (!playerExist)
            return Error.Failure("Не найден игрок");
        if (!VoteStarted)
            return Error.Failure("Голосование не начато");
        if (!VotingIssueId.HasValue)
            return Error.Failure("Не выбран объект голосования");
        var issue = _issues.GetValueOrDefault(VotingIssueId.Value);
        if (issue is null)
            return Error.Failure("Не найден объект голосования");
        issue.PlayerStoryPoints[request.PlayerId] = request.StoryPoints;
        return Result.Success;
    }

    public void RemoveIssue(Guid issueId) => _issues.Remove(issueId);
}
