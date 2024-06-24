using StoryPoker.Server.Abstractions.DefaultRoom.Models.Enums;
using StoryPoker.Server.Abstractions.GroupedRoom.Models;
using StoryPoker.Server.Grains.GroupedRoom.Models;

namespace StoryPoker.Server.Grains.Extensions;

public static class InternalGroupedRoomExtensions
{
    public static GroupedRoomResponse ToResponse(this InternalGroupedRoom internalState, Guid playerId)
    {
        return new GroupedRoomResponse
        {
            Name = internalState.Name,
            Player = GetPlayer(internalState, playerId),
            Spectator = GetSpectator(internalState, playerId),
            VotingIssue = GetVotingIssue(internalState),
            Groups = GetGroups(internalState, playerId),
            Issues = GetIssues(internalState),
            IssueOrder = internalState.IssueOrderBy
        };
    }

    private static IReadOnlyCollection<GroupedIssueStateResponse> GetIssues(InternalGroupedRoom internalState)
    {
        var issues = internalState.Issues.Select(issue => new GroupedIssueStateResponse
        {
            Id = issue.Key,
            Title = issue.Value.Name,
            Stage = issue.Value.Stage,
            Order = issue.Value.Order,
            GroupPoints = GetGroupPoints(internalState, issue.Value)
        });
        switch (internalState.IssueOrderBy)
        {
            default:
            case IssueOrder.Asc:
                issues = issues.OrderBy(x => x.Order);
                break;
            case IssueOrder.Desc:
                issues = issues.OrderByDescending(x => x.Order);
                break;
        }

        return issues.ToList();
    }

    private static IReadOnlyCollection<GroupStateResponse> GetGroups(InternalGroupedRoom internalState, Guid playerId)
    {
        if (internalState.Players.TryGetValue(playerId, out var player))
        {
            var playerGroup = new GroupStateResponse()
            {
                Name = internalState.Groups[player.GroupId].Name,
                Players = GetPlayers(internalState, internalState.Groups[player.GroupId], playerId)
            };
            var otherGroups =  internalState.Groups
                .Where(x => x.Key != player.GroupId)
                .OrderBy(x => x.Value.Order)
                .Select(group => new GroupStateResponse
                {
                    Name = group.Value.Name,
                    Players = GetPlayers(internalState,group.Value, playerId)
                });
            var result = new List<GroupStateResponse> { playerGroup };
            result.AddRange(otherGroups);
            return result;
        }

        return internalState.Groups
            .OrderBy(x => x.Value.Order)
            .Select(group => new GroupStateResponse
        {
            Name = group.Value.Name,
            Players = GetPlayers(internalState,group.Value, playerId)
        }).ToList();
    }

    private static IReadOnlyCollection<GroupedPlayerStateResponse> GetPlayers(InternalGroupedRoom internalState, InternalGroup group, Guid playerId)
    {
        return group.Players.Select(player =>
        {
            var voted = internalState.VotingIssue?.PlayerPoints.ContainsKey(player.Key) ?? false;
            var isCurrentPlayer = player.Key == playerId;
            var showValue = (isCurrentPlayer && voted) || (internalState.VotingIssue?.Stage == VotingStage.VoteEnded && voted);
            var value = showValue ? internalState.VotingIssue?.PlayerPoints[player.Key] : null;
            return new GroupedPlayerStateResponse
            {
                Id = player.Key,
                Name = player.Value.Name,
                IsCurrentPlayer = isCurrentPlayer,
                VotingStage = internalState.VotingIssue?.Stage,
                VotingState = new() { Voted = voted, Value = value }
            };
        }).ToList();
    }

    private static GroupedIssueStateResponse? GetVotingIssue(InternalGroupedRoom internalState)
    {
        if (internalState.VotingIssue is null)
            return null;
        var issue = internalState.VotingIssue;
        return new()
        {
            Id = issue.Id,
            Title = issue.Name,
            Stage = issue.Stage,
            Order = issue.Order,
            GroupPoints = GetGroupPoints(internalState, issue)
        };
    }

    private static List<GroupStoryPoints> GetGroupPoints(InternalGroupedRoom internalState, InternalGroupedIssue issue)
    {
        return issue.CalculatedGroupPoints.Select(group => new GroupStoryPoints
        {
            Name = internalState.Groups[group.Key].Name,
            StoryPoints = group.Value.StoryPoints,
            FibonacciStoryPoints = group.Value.FibonacciStoryPoints
        }).ToList();
    }

    private static GroupedSpectatorStateResponse GetSpectator(InternalGroupedRoom internalState, Guid playerId)
    {
        return new()
        {
            Id = internalState.Spectator.Id,
            Name = internalState.Spectator.Name,
            IsCurrentPlayer = internalState.Spectator.Id == playerId,
            InRoom = internalState.Spectator?.InRoom ?? false
        };
    }

    private static GroupedCurrentPlayerResponse GetPlayer(InternalGroupedRoom internalState, Guid playerId)
    {
        if(internalState.Spectator?.Id == playerId)
            return new() { Id = playerId, IsAdded = true, CanVote = false };
        if (!internalState.Players.TryGetValue(playerId, out var player))
            return new() { Id = playerId, IsAdded = false, CanVote = false };
        var canVote = internalState.VotingIssue is not null
                      && internalState.VotingIssue.CalculatedGroupPoints.ContainsKey(player.GroupId);
        return new() { Id = playerId, IsAdded = true, CanVote = canVote };
    }
}
