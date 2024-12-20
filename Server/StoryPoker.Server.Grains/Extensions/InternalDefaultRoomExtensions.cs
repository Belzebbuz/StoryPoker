﻿using StoryPoker.Server.Abstractions.DefaultRoom.Models;
using StoryPoker.Server.Abstractions.DefaultRoom.Models.Enums;
using StoryPoker.Server.Grains.DefaultRoom.Models;

namespace StoryPoker.Server.Grains.Extensions;

public static class InternalDefaultRoomExtensions
{
    public static DefaultRoomStateResponse ToResponse(this InternalDefaultRoom internalRoom, Guid playerId)
    {
        return new DefaultRoomStateResponse()
        {
            PlayerId = playerId,
            IsPlayerAdded = internalRoom.Players.ContainsKey(playerId),
            Name = internalRoom.Name,
            VotingIssue = GetVotingIssue(internalRoom),
            IsSpectator = internalRoom.Players.TryGetValue(playerId, out var player) && player.IsSpectator,
            Players = GetPlayers(playerId,internalRoom),
            Issues = GetIssues(internalRoom),
            IssueOrder = internalRoom.IssueOrderBy,
            SpectatorCanVote = internalRoom.SpectatorCanVote,
            SkipBorderValues = internalRoom.SkipBorderValues
        };
    }

    private static ICollection<IssueStateResponse> GetIssues(InternalDefaultRoom state)
    {
        var issues = state.Issues.Values.AsEnumerable();
        issues = state.IssueOrderBy switch
        {
            IssueOrder.Asc => issues.OrderBy(x => x.Order),
            IssueOrder.Desc => issues.OrderByDescending(x => x.Order),
            _ => issues.OrderBy(x => x.Order)
        };

        return issues.Select(issueState =>
            new IssueStateResponse()
            {
                Id = issueState.Id,
                Order = issueState.Order,
                Stage = issueState.Stage,
                Title = issueState.Title,
                StoryPoints = issueState.StoryPoints,
                FibonacciStoryPoints = issueState.FibonacciStoryPoints
            }).ToArray();
    }

    private static ICollection<PlayerStateResponse> GetPlayers(Guid userId, InternalDefaultRoom state) =>
        state.Players.Values
            .OrderBy(x => x.Order)
            .Select(
                playerState =>
                {
                    var playerVoted =
                        state.VotingIssue?.PlayerStoryPoints
                            .ContainsKey(playerState.Id) ?? false;
                    var canShowValue =
                        (state.VotingIssue?.Stage != VotingStage.Voting && state.VotingIssue?.Stage != VotingStage.VoteEnding)
                        || userId == playerState.Id;
                    var playerVotePoint = playerVoted && canShowValue
                        ? state.VotingIssue?.PlayerStoryPoints[playerState.Id]
                        : null;
                    return new PlayerStateResponse()
                    {
                        Id = playerState.Id,
                        Name = playerState.Name,
                        IsSpectator = playerState.IsSpectator,
                        CurrentVotingPoint = new PlayerIssueStoryPoint()
                        {
                            Value = playerVotePoint,
                            Voted = playerVoted
                        }
                    };
                }).ToArray();


    private static IssueStateResponse? GetVotingIssue(InternalDefaultRoom state)
    {
        if (!state.VotingIssueId.HasValue)
            return null;
        var issueState = state.Issues[state.VotingIssueId.Value];
        return  new IssueStateResponse()
        {
            Id = issueState.Id,
            Order = issueState.Order,
            Stage = issueState.Stage,
            Title = issueState.Title,
            StoryPoints = issueState.StoryPoints,
            FibonacciStoryPoints = issueState.FibonacciStoryPoints
        };
    }
}
