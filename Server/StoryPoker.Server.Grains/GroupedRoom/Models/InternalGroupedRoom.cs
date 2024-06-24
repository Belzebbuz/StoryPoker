using ErrorOr;
using Newtonsoft.Json;
using StoryPoker.Server.Abstractions.DefaultRoom.Commands;
using StoryPoker.Server.Abstractions.DefaultRoom.Models.Enums;
using StoryPoker.Server.Abstractions.GroupedRoom.State;
using StoryPoker.Server.Abstractions.Metadata.Models;
using StoryPoker.Server.Grains.Abstractions;
using StoryPoker.Server.Grains.DefaultRoom.Models.DomainEvents;
using StoryPoker.Server.Grains.GroupedRoom.Models.DomainEvents;

namespace StoryPoker.Server.Grains.GroupedRoom.Models;

public record InternalGroupedRoom : AggregateRoot, IGroupedRoomState
{
    [JsonProperty] public string Name { get; private set; } = string.Empty;
    [JsonProperty] public bool Instantiated { get; private set; }
    public required InternalGroupedSpectator Spectator { get; init; }
    [JsonProperty] public Guid? VotingIssueId { get; private set; }
    [JsonProperty] public IssueOrder IssueOrderBy { get; private set; }
    [JsonIgnore] public InternalGroupedIssue? VotingIssue => VotingIssueId.HasValue ? _issues[VotingIssueId.Value] : null;

    public IReadOnlyDictionary<string, InternalGroup> Groups => _groups;
    private readonly Dictionary<string, InternalGroup> _groups = new();

    public IReadOnlyDictionary<Guid, InternalGroupedIssue> Issues => _issues;
    private readonly Dictionary<Guid, InternalGroupedIssue> _issues = new();

    [JsonIgnore] public IReadOnlyDictionary<Guid, InternalGroupedPlayer> Players =>
        _groups.Values
            .SelectMany(x => x.Players)
            .ToDictionary()
            .AsReadOnly();
    public static InternalGroupedRoom Init(CreateRoomRequest request)
    {
        var spectator = new InternalGroupedSpectator()
        {
            Name = request.PlayerName,
            Id = request.PlayerId,
            InRoom = true
        };
        var state = new InternalGroupedRoom()
        {
            Name = request.RoomName,
            Instantiated = true,
            Spectator = spectator,
        };
        state.AddGroupRange(request.GroupNames);
        return state;
    }

    public ErrorOr<Success> AddGroupRange(ICollection<string> groupNames)
    {
        foreach (var groupName in groupNames)
        {
            if(_groups.ContainsKey(groupName))
                continue;
            var order = _groups.Values.Count == 0 ? 1 : _groups.Values.Max(x => x.Order) + 1;
            var group = new InternalGroup { Name = groupName, Order = order };
            _groups.Add(groupName,group);
        }

        return Result.Success;
    }

    public ErrorOr<Success> RemovePlayer(Guid userId)
    {
        if (Spectator.Id == userId)
        {
            Spectator.InRoom = false;
            return Result.Success;
        }
        if(!Players.TryGetValue(userId, out var player))
            return Error.Failure(description: "Игрок не найден");
        return Groups[player.GroupId].RemovePlayer(userId);
    }
    public ErrorOr<Success> AddPlayer(Guid userId, string playerName, string groupName)
    {
        if (Spectator.Id == userId)
        {
            Spectator.InRoom = true;
            return Result.Success;
        }
        if (!_groups.TryGetValue(groupName, out var group))
            return Error.Failure(description: "Команда не найдена");
        var player = new InternalGroupedPlayer { Id = userId, Name = playerName, GroupId = groupName };
        return group.AddPlayer(player);
    }
    public ErrorOr<Success> RenameGroup(string oldName, string newName)
    {
        if (!_groups.TryGetValue(oldName, out var group))
            return Error.Failure(description: "Команда с таким именем не найдена");
        if (_groups.ContainsKey(newName))
            return Error.Failure(description: "Команда с таким именем уже существует");
        _groups.Remove(oldName);
        _groups[newName] = group with {Name = newName};
        var issues = _issues
            .Where(x => x.Value.CalculatedGroupPoints.ContainsKey(oldName))
            .Select(x => x.Value);
        foreach (var issue in issues)
        {
            issue.RenameGroup(oldName, newName);
        }

        foreach (var player in group.Players.Values)
        {
            player.GroupId = newName;
        }
        return Result.Success;
    }

    public ErrorOr<Success> RemoveGroup(string groupName)
    {
        if(!_groups.TryGetValue(groupName, out var group))
            return Error.Failure(description: "Команда с таким именем не найдена");
        if(group.Players.Any())
            return Error.Failure(description: "Невозможно удалить команду с игроками");
        var anyLastInIssue = _issues.Values.Any(x =>
            x.CalculatedGroupPoints.ContainsKey(groupName) && x.CalculatedGroupPoints.Count == 1);
        if(anyLastInIssue)
            return Error.Failure(description: "Нельзя удалить команду. Задача должна быть связана хотя бы с одной командой.");
        var issues = _issues
            .Where(x => x.Value.CalculatedGroupPoints.ContainsKey(groupName))
            .Select(x => x.Value);
        foreach (var issue in issues)
        {
            issue.RemoveGroup(groupName);
        }
        _groups.Remove(groupName);
        return Result.Success;
    }

    public ErrorOr<Success> ChangePlayerGroup(Guid playerId, string groupName)
    {
        if(!Players.TryGetValue(playerId, out var player))
            return Error.Failure(description: "Игрок не найден");
        if(player.GroupId == groupName)
            return Result.Success;
        if(!_groups.TryGetValue(groupName, out var newGroup))
            return Error.Failure(description: "Команда с таким именем не найдена");

        if (!_groups.TryGetValue(player.GroupId, out var oldGroup))
            return Error.Failure(description: "Текущая команда игрока не найдена");
        var removeResult = oldGroup.RemovePlayer(playerId);
        if (removeResult.IsError)
            return removeResult;
        var issuesWithPlayer = _issues.Where(x => x.Value.PlayerPoints.ContainsKey(playerId));
        foreach (var (_, issue) in issuesWithPlayer)
        {
            issue.RemovePlayer(playerId);
        }
        var addResult = newGroup.AddPlayer(player);
        if (addResult.IsError)
            return addResult;
        return Result.Success;
    }

    public ErrorOr<Success> AddIssue(string title, string[] groupNames)
    {
        if(!groupNames.Any(group => _groups.ContainsKey(group)))
            return Error.Failure(description: "Команда не найдена");
        var order = _issues.Count != 0 ? _issues.Values.Max(x => x.Order) + 1 : 1;
        var issue = new InternalGroupedIssue
        {
            Id = Guid.NewGuid(), Name = title, Stage = VotingStage.NotStarted, Order = order
        };
        _issues.Add(issue.Id, issue);
        if (VotingIssue is null)
            VotingIssueId = issue.Id;
        issue.AddGroupRange(groupNames);
        return Result.Success;
    }

    public ErrorOr<Success> UpdateIssue(Guid issueId, string title, string[] groupNames)
    {
        if (!_issues.TryGetValue(issueId, out var issue))
            return Error.Failure(description:"Не удалось найти задачу");
        if(issue.IsVoting)
            return Error.Failure(description:"Невозможно изменить задачу");
        var updateResult = issue.Update(title, groupNames);
        if (updateResult.IsError)
            return updateResult;
        return Result.Success;
    }

    public ErrorOr<Success> RemoveIssue(Guid issueId)
    {
        if (!_issues.TryGetValue(issueId, out var issue))
            return Error.Failure(description: "Задача не найдена или уже удалена");
        if (issue.IsVoting)
            return Error.Failure(description: "Невозможно удалить задачу, по которой идет голосование");
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

    public ErrorOr<Success> SetVotingIssue(Guid issueId)
    {
        if (VotingIssue?.Id == issueId)
            return Result.Success;
        if (VotingIssue is not null && VotingIssue.IsVoting)
            return Error.Failure(description: "Невозможно изменить задачу во время голосования");
        if(!_issues.ContainsKey(issueId))
            return Error.Failure(description: "Задача не найдена или уже удалена");
        VotingIssueId = issueId;
        return Result.Success;
    }

    public ErrorOr<Success> ChangeVotingStage(VoteStageChangeType votingStage)
    {
        if (VotingIssue is null)
            return Error.Failure(description: "Не выбран объект голосования");
        switch (votingStage)
        {
            case VoteStageChangeType.Start:
                return VotingIssue.StartVote();
            case VoteStageChangeType.StartEndTimer:
                var result = VotingIssue.StartEndingVote();
                if (result.IsError)
                    return result;
                AddEvent(new GroupedVoteEndingTimerEvent(VotingIssue.Id));
                return Result.Success;
            case VoteStageChangeType.Stop:
                return VotingIssue.StopVote();
            default:
                return Error.Failure("Команда не распознана");
        }
    }

    public ErrorOr<Success> SetStoryPoint(Guid playerId, int storyPoint)
    {
        if (!Players.TryGetValue(playerId, out var player))
            return Error.Failure(description: "Не найден игрок");
        if (VotingIssue is null)
            return Error.Failure(description:"Не выбран объект голосования");
        VotingIssue.SetStoryPoint(player, storyPoint);
        if (VotingIssue.Stage != VotingStage.Voting)
            return Result.Success;

        var votedCount = VotingIssue.GroupPoints.Values.SelectMany(x => x.Values).Count();
        var totalCount = VotingIssue.CalculatedGroupPoints.Keys.Sum(groupName => _groups[groupName].Players.Count);
        if (totalCount != votedCount)
            return Result.Success;

        VotingIssue.StartEndingVote();
        AddEvent(new GroupedVoteEndingTimerEvent(VotingIssue.Id));
        return Result.Success;
    }

    public ErrorOr<Success> SetIssuesOrderBy(IssueOrder orderBy)
    {
        IssueOrderBy = orderBy;
        return Result.Success;
    }

    public ErrorOr<Success> SetIssueOrder(Guid issueId, int newOrder)
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
}
