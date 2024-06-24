using ErrorOr;
using Newtonsoft.Json;
using StoryPoker.Server.Abstractions.DefaultRoom.Models;
using StoryPoker.Server.Abstractions.DefaultRoom.Models.Enums;
using StoryPoker.Server.Abstractions.DefaultRoom.State;
using StoryPoker.Server.Abstractions.Metadata.Models;
using StoryPoker.Server.Grains.Abstractions;
using StoryPoker.Server.Grains.DefaultRoom.Models.DomainEvents;

namespace StoryPoker.Server.Grains.DefaultRoom.Models;

public record InternalDefaultRoom : AggregateRoot, IRoomState
{
    [JsonProperty] public string Name { get; private set; } = string.Empty;
    public IReadOnlyDictionary<Guid, InternalDefaultPlayer> Players => _players; // 1 - TAG
    private readonly Dictionary<Guid, InternalDefaultPlayer> _players = new();
    public IReadOnlyDictionary<Guid, InternalDefaultIssue> Issues => _issues; // MANY - TAGS TAG -> ОЦЕНКА
    private readonly Dictionary<Guid, InternalDefaultIssue> _issues = new();
    [JsonProperty] public Guid? VotingIssueId { get; private set; }
    [JsonProperty] public bool Instantiated { get; private set; }
    [JsonProperty] public IssueOrder IssueOrderBy { get; private set; }
    [JsonIgnore] public InternalDefaultIssue? VotingIssue => VotingIssueId.HasValue ? _issues[VotingIssueId.Value] : null;
    public static InternalDefaultRoom Init(CreateRoomRequest request)
    {
        var playerState = new InternalDefaultPlayer() { Name = request.PlayerName, IsSpectator = true, Id = request.PlayerId, Order = 1 };
        var state = new InternalDefaultRoom()
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
    public ErrorOr<Success> SetIssueOrderBy(IssueOrder order)
    {
        IssueOrderBy = order;
        return Result.Success;
    }

    public ErrorOr<Success> AddNewPlayer(DefaultRoomRequest.AddPlayer request)
    {
        var playerExist = _players.ContainsKey(request.Id);
        if (playerExist)
            return Result.Success;
        var order = _players.Count != 0 ? _players.Values.Max(x => x.Order) + 1 : 1;
        var player = new InternalDefaultPlayer()
        {
            Id = request.Id,
            Name = request.Name,
            IsSpectator = _players.Count == 0,
            Order = order
        };
        AddPlayer(player);
        return Result.Success;
    }

    public ErrorOr<Success> RemovePlayer(Guid id)
    {
        var player = _players.GetValueOrDefault(id);
        if(player is null)
            return Result.Success;
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

        return Result.Success;
    }

    public ErrorOr<Success> SetNewSpectator(Guid playerId)
    {
        if (VotingIssue is not null && VotingIssue.Stage == VotingStage.Voting)
            return Error.Failure(description: "Нельзя поменять ведущего во время голосования");
        var currentSpectator = _players.Values.FirstOrDefault(x => x.IsSpectator);
        if(currentSpectator is not null)
            currentSpectator.IsSpectator = false;
        var nextSpectator = _players[playerId];
        nextSpectator.IsSpectator = true;
        return Result.Success;
    }

    private void AddPlayer(InternalDefaultPlayer player)
    {
        _players.Add(player.Id, player);
    }

    public ErrorOr<Success> StartVote()
    {
        if (VotingIssue is null)
            return Error.Failure(description: "Не выбран объект голосования");
        return VotingIssue.StartVote();
    }

    public ErrorOr<Success> StopVote()
    {
        if (VotingIssue is null)
            return Error.Failure(description: "Не выбран объект голосования");
        if (VotingIssue.Stage == VotingStage.VoteEnded)
            return Result.Success;
        VotingIssue.StopVote();
        return Result.Success;
    }

    public ErrorOr<Success> SetEndingTimerVote()
    {
        if (VotingIssue is null)
            return Error.Failure(description: "Не выбран объект голосования");
        if(VotingIssue.Stage != VotingStage.Voting)
            return Error.Failure(description: "По данной задаче не идет голосование");
        VotingIssue.StartEndingVote();
        AddEvent(new VoteEndingTimerEvent(VotingIssue.Id));
        return Result.Success;
    }

    public ErrorOr<Success> AddIssue(DefaultRoomRequest.AddIssue addIssueRequest)
    {
        var order = _issues.Count != 0 ? _issues.Values.Max(x => x.Order) + 1 : 1;
        var issue = new InternalDefaultIssue()
        {
            Id = Guid.NewGuid(),
            Title = addIssueRequest.Title,
            Order = order,
            Stage = VotingStage.NotStarted
        };
        _issues.Add(issue.Id, issue);
        if (VotingIssue is not null)
        {
            if(VotingIssue.Stage != VotingStage.Voting)
                VotingIssueId = issue.Id;
            return Result.Success;
        }
        VotingIssueId = issue.Id;
        return Result.Success;
    }

    public ErrorOr<Success> SetCurrentIssue(Guid issueId)
    {
        var issue = _issues.GetValueOrDefault(issueId);
        if (issue is null)
            return Error.Failure(description:"Не найдена задача");
        if (VotingIssue is not null && !VotingIssue.CanChangeVotingIssue())
        {
          return Error.Failure(description:"Голосование по текущей задаче еще не закончилось");
        }
        VotingIssueId = issueId;
        return Result.Success;
    }
    public ErrorOr<Success> SetStoryPoint(DefaultRoomRequest.SetStoryPoint request)
    {
        var playerExist = _players.ContainsKey(request.PlayerId);
        if (!playerExist)
            return Error.Failure(description:"Не найден игрок");
        if (VotingIssue is null)
            return Error.Failure(description:"Не выбран объект голосования");
        if (VotingIssue.Stage == VotingStage.NotStarted)
            return Error.Failure(description:"Голосование не начато");
        VotingIssue.PlayerStoryPoints[request.PlayerId] = request.StoryPoints;

        if (VotingIssue.Stage == VotingStage.VoteEnded)
        {
            VotingIssue.RecalculateStoryPoints();
            return Result.Success;
        }
        if (VotingIssue.PlayerStoryPoints.Count == _players.Count - 1 && VotingIssue.Stage == VotingStage.Voting)
        {
            SetEndingTimerVote();
        }
        return Result.Success;
    }

    public ErrorOr<Success> RemoveIssue(Guid issueId)
    {
        if(_issues[issueId].CanRemove())
            return Error.Failure(description: "Невозможно удалить предмет голосования");
        _issues.Remove(issueId);
        if(VotingIssueId.HasValue && VotingIssueId.Value == issueId)
            VotingIssueId = null;
        var order = 1;
        foreach (var issueState in _issues.Values.OrderBy(x => x.Order))
        {
            issueState.Order = order;
            order++;
        }
        return Result.Success;
    }
}
