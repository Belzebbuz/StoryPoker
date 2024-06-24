using ErrorOr;
using Newtonsoft.Json;
using StoryPoker.Server.Abstractions.DefaultRoom.Models.Enums;
using StoryPoker.Server.Grains.Constants;

namespace StoryPoker.Server.Grains.GroupedRoom.Models;

public record InternalGroupedIssue
{
    public required Guid Id { get; init; }
    public required string Name { get; set; }
    public required VotingStage Stage { get; set; }
    public required int Order { get; set; }
    public IReadOnlyDictionary<string, IDictionary<Guid, int>> GroupPoints => _groupPoints;
    private readonly Dictionary<string, IDictionary<Guid, int>> _groupPoints = new();

    public IReadOnlyDictionary<string, InternalGroupedIssuePoints> CalculatedGroupPoints => _calculatedGroupPoints;
    private Dictionary<string, InternalGroupedIssuePoints> _calculatedGroupPoints = new();

    [JsonIgnore] public IReadOnlyDictionary<Guid, int> PlayerPoints => _groupPoints.SelectMany(group => group.Value).ToDictionary();

    [JsonIgnore] internal bool IsVoting => Stage is VotingStage.Voting or VotingStage.VoteEnding;
    public ErrorOr<Success> AddGroupRange(string[] groupNames)
    {
        foreach (var groupName in groupNames)
        {
            if (!_calculatedGroupPoints.ContainsKey(groupName))
            {
                _calculatedGroupPoints.Add(groupName, new InternalGroupedIssuePoints());
            }
        }

        return Result.Success;
    }

    public ErrorOr<Success> RemoveGroup(string groupName)
    {
        _calculatedGroupPoints.Remove(groupName);
        _groupPoints.Remove(groupName);
        return Result.Success;
    }

    public ErrorOr<Success> RenameGroup(string oldName, string newName)
    {
        if (!_calculatedGroupPoints.TryGetValue(oldName, out var group))
            return Error.Failure(description: "Команда не найдена");
        if(_calculatedGroupPoints.ContainsKey(newName))
            return Error.Failure(description: "Команда с таким именем уже существует");
        _calculatedGroupPoints.Remove(oldName);
        _calculatedGroupPoints[newName] = group;
        if (!_groupPoints.Remove(oldName, out var groupPoints))
            return Result.Success;
        _groupPoints[newName] = groupPoints;
        return Result.Success;
    }
    public ErrorOr<Success> Update(string title, string[] groupNames)
    {
        Name = title;
        AddGroupRange(groupNames);
        var excluded = _calculatedGroupPoints.Keys.Except(groupNames);
        foreach (var excludedGroup in excluded)
        {
            RemoveGroup(excludedGroup);
        }
        return Result.Success;
    }
    public ErrorOr<Success> StartVote()
    {
        if (Stage == VotingStage.Voting)
            return Error.Failure(description: "Голосование уже началось");
        _groupPoints.Clear();
        Stage = VotingStage.Voting;
        return Result.Success;
    }

    public ErrorOr<Success> StartEndingVote()
    {
        if (Stage != VotingStage.Voting)
            return Error.Failure(description: "Голосование еще не началось");
        Stage = VotingStage.VoteEnding;
        return Result.Success;
    }

    public ErrorOr<Success> StopVote()
    {
        if (Stage != VotingStage.VoteEnding)
            return Error.Failure(description: "Таймер остановки не был запущен");

        Stage = VotingStage.VoteEnded;
        RecalculateStoryPoints();
        return Result.Success;
    }
    private void RecalculateStoryPoints()
    {
        foreach (var (groupName, points) in _calculatedGroupPoints)
        {
           var storyPoints = CalculateStoryPoints(groupName);
           var fibonacci = CalculateFibonacciStoryPoints(storyPoints);
           points.Update(storyPoints, fibonacci);
        }
    }
    private float? CalculateStoryPoints(string groupName) =>
        (!_groupPoints.TryGetValue(groupName, out var playersStoryPoints) || playersStoryPoints.Count == 0)
        ? null
        : (float)Math.Round(playersStoryPoints.Average(x => x.Value),1);

    private static int? CalculateFibonacciStoryPoints(float? storyPoints)
    {
        if (!storyPoints.HasValue)
            return null;
        foreach (var fib in Fibonacci.Sequence.Where(fib => fib >= storyPoints.Value))
        {
            return fib;
        }

        return Fibonacci.Sequence[^1];
    }

    public ErrorOr<Success> SetStoryPoint(InternalGroupedPlayer player, int storyPoint)
    {
        if (Stage == VotingStage.NotStarted)
            return Error.Failure(description:"Голосование не начато");
        if(!_calculatedGroupPoints.ContainsKey(player.GroupId))
            return Error.Failure(description:$"Группа {player.GroupId} не участвует в голосовании");
        if (!_groupPoints.TryGetValue(player.GroupId, out var group))
        {
            group = new Dictionary<Guid, int>();
            _groupPoints[player.GroupId] = group;
        }

        group[player.Id] = storyPoint;
        if(Stage == VotingStage.VoteEnded)
            RecalculateStoryPoints();
        return Result.Success;
    }

    public void RemovePlayer(Guid playerId)
    {
        var groupWithPlayers = _groupPoints.Where(x => x.Value.ContainsKey(playerId));
        foreach (var (key, players) in groupWithPlayers)
        {
            players.Remove(playerId);
        }
    }
}
