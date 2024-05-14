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

    public static RoomGrainState Init(RoomRequest.CreateRoom request)
    {
        var playerState = new PlayerState() { Name = request.PlayerName, IsSpectator = true, Id = request.PlayerId, Order = 1 };
        var state = new RoomGrainState()
        {
            Name = request.RoomName,
            Instantiated = true
        };
        state.AddPlayer(playerState);
        return state;
    }

    public ErrorOr<Success> AddNewPlayer(RoomRequest.AddPlayer request)
    {
        var playerExist =_players.ContainsKey(request.Id);
        if (playerExist)
            return Result.Success;
        var order = _players.Count != 0 ? _players.Values.Max(x => x.Order) + 1 : 1;
        var player = new PlayerState()
        {
            Id = request.Id,
            Name = request.Name,
            IsSpectator = _players.Count == 0,
            Order = order
        };
        AddPlayer(player);
        return Result.Success;
    }

    public void RemovePlayer(Guid id)
    {
        var player = _players.GetValueOrDefault(id);
        if(player is null)
            return;
        _players.Remove(id);
        if (player.IsSpectator && _players.Count != 0)
        {
            var nextSpectator = _players.Values.OrderBy(x => x.Order).First();
            nextSpectator.IsSpectator = true;
        }

        if (VotingIssueId.HasValue && _issues[VotingIssueId.Value].PlayerStoryPoints.ContainsKey(player.Id))
        {
            _issues[VotingIssueId.Value].PlayerStoryPoints.Remove(player.Id);
        }
    }

    private void AddPlayer(PlayerState player)
    {
        _players.Add(player.Id, player);
    }

    public ErrorOr<Success> StartVote()
    {
        if (!VotingIssueId.HasValue)
            return Error.Failure(description: "Не выбран объект голосвания");
        _issues[VotingIssueId.Value].PlayerStoryPoints.Clear();
        var issue = _issues[VotingIssueId.Value];
        if (issue.Stage == VotingStage.Voting)
            return Error.Failure(description: "Голосвание уже началось");
        issue.Stage = VotingStage.Voting;
        return Result.Success;
    }

    public ErrorOr<Success> StopVote()
    {
        if (!VotingIssueId.HasValue)
            return Error.Failure(description: "Не выбран объект голосвания");

        var issue = _issues[VotingIssueId.Value];
        if (issue.Stage != VotingStage.Voting)
            return Error.Failure(description: "Голосование не начато");

        issue.Stage = VotingStage.VoteEnded;
        issue.StoryPoints = issue.PlayerStoryPoints.Count == 0
            ? null
            : (int)Math.Round(issue.PlayerStoryPoints.Average(x => x.Value), MidpointRounding.AwayFromZero);
        return Result.Success;
    }

    public void AddIssue(RoomRequest.AddIssue addIssueRequest)
    {
        var order = _issues.Count != 0 ? _issues.Values.Max(x => x.Order) + 1 : 1;
        var issue = new IssueState()
        {
            Id = Guid.NewGuid(),
            Title = addIssueRequest.Title,
            Order = order,
            Stage = VotingStage.NotStarted
        };
        _issues.Add(issue.Id, issue);
        if (VotingIssueId.HasValue)
        {
            var votingIssue = _issues[VotingIssueId.Value];
            if(votingIssue.Stage != VotingStage.Voting)
                VotingIssueId = issue.Id;
            return;
        }
        VotingIssueId = issue.Id;
    }

    public ErrorOr<Success> SetCurrentIssue(Guid issueId)
    {
        var issue = _issues.GetValueOrDefault(issueId);
        if (issue is null)
            return Error.Failure(description:"Не найдена задача");
        if (VotingIssueId.HasValue)
        {
            var currentIssue = _issues[VotingIssueId.Value];
            if (currentIssue.Stage == VotingStage.Voting)
                return Error.Failure(description:"Голосование по текущей задаче еще не закончилось");
        }
        VotingIssueId = issueId;
        return Result.Success;
    }

    public ErrorOr<Success> SetStoryPoint(RoomRequest.SetStoryPoint request)
    {
        var playerExist = _players.ContainsKey(request.PlayerId);
        if (!playerExist)
            return Error.Failure(description:"Не найден игрок");
        if (!VotingIssueId.HasValue)
            return Error.Failure(description:"Не выбран объект голосования");
        var issue = _issues[VotingIssueId.Value];
        if (issue.Stage != VotingStage.Voting)
            return Error.Failure(description:"Голосование не начато");
        issue.PlayerStoryPoints[request.PlayerId] = request.StoryPoints;
        return issue.PlayerStoryPoints.Count == _players.Count - 1 ? StopVote() : Result.Success;
    }

    public ErrorOr<Success> RemoveIssue(Guid issueId)
    {
        if(VotingIssueId.HasValue && VotingIssueId.Value == issueId)
            return Error.Failure(description: "Невозможно удалить предмет голосования");
        _issues.Remove(issueId);
        return Result.Success;
    }
}

