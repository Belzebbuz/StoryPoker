using ErrorOr;
using Newtonsoft.Json;
using OneOf.Types;
using Error = ErrorOr.Error;
using Success = ErrorOr.Success;

namespace StoryPoker.Server.Abstractions.Room.Models;

[GenerateSerializer, Immutable]
public record RoomGrainState
{
    [Id(1)] public required string Name { get; init; }
    public IReadOnlyDictionary<Guid, PlayerState> Players => _players;
    [Id(2)] private readonly Dictionary<Guid, PlayerState> _players = new();
    public IReadOnlyDictionary<Guid, IssueState> Issues => _issues;
    [Id(3)] private readonly Dictionary<Guid, IssueState> _issues = new();
    [Id(4), JsonProperty] public Guid? VotingIssueId { get; private set; }
    [Id(5), JsonProperty] public bool Instantiated { get; private set; }
    [Id(6), JsonProperty] public IssueOrder IssueOrderBy { get; private set; }
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

    public ErrorOr<Success> SetIssuesNewOrder(Guid issueId, int newOrder)
    {
        if (!_issues.TryGetValue(issueId, out var issue))
            return Error.Failure(description: "Нет такой задачи");
        if (newOrder < 1 || newOrder > _issues.Count)
            return Error.Failure(description: "Новый порядковый номер выходит за допустимые границы");
        var oldOrder = issue.Order;
        if (newOrder == oldOrder)
            return Result.Success;
        foreach (var (id, issueState) in _issues)
        {
            if (newOrder < oldOrder && issueState.Order >= newOrder && issueState.Order < oldOrder)
                issueState.Order++;
            else if (newOrder > oldOrder && issueState.Order <= newOrder && issueState.Order > oldOrder)
                issueState.Order--;
        }
        issue.Order = newOrder;
        return Result.Success;
    }
    public void SetIssueOrderBy(IssueOrder order) => IssueOrderBy = order;

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

    public ErrorOr<Success> SetNewSpectator(Guid playerId)
    {
        if (VotingIssueId.HasValue && _issues[VotingIssueId.Value].Stage == VotingStage.Voting)
            return Error.Failure(description: "Нельзя поменять ведущего во время голосования");
        var currentSpectator = _players.Values.FirstOrDefault(x => x.IsSpectator);
        if(currentSpectator is not null)
            currentSpectator.IsSpectator = false;
        var nextSpectator = _players[playerId];
        nextSpectator.IsSpectator = true;
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
        if(_issues[issueId].Stage == VotingStage.Voting)
            return Error.Failure(description: "Невозможно удалить предмет голосования");
        _issues.Remove(issueId);
        VotingIssueId = null;
        return Result.Success;
    }
}

