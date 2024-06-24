using StoryPoker.Server.Abstractions.Common;
using StoryPoker.Server.Abstractions.DefaultRoom.Models.Enums;

namespace StoryPoker.Server.Abstractions.GroupedRoom.Models;

[GenerateSerializer, Immutable]
public class GroupedRoomResponse : IRoomStateResponse
{
    [Id(0)]public required string Name { get; init; }
    [Id(1)]public required GroupedCurrentPlayerResponse Player { get; init; }
    [Id(2)]public required GroupedSpectatorStateResponse? Spectator { get; init; }
    [Id(3)]public required GroupedIssueStateResponse? VotingIssue { get; init; }
    [Id(4)]public required IReadOnlyCollection<GroupStateResponse> Groups { get; init; }
    [Id(5)]public required IReadOnlyCollection<GroupedIssueStateResponse> Issues { get; init; }
    [Id(6)]public required IssueOrder IssueOrder { get; init;  }
}

[GenerateSerializer,Immutable]
public class GroupStateResponse
{
    [Id(0)] public required string Name { get; init; }
    [Id(1)] public required IReadOnlyCollection<GroupedPlayerStateResponse> Players { get; init; }
}

[GenerateSerializer,Immutable]
public class GroupedIssueStateResponse
{
    [Id(0)] public required Guid Id { get; init; }
    [Id(1)] public required string Title { get; init; }
    [Id(2)] public required VotingStage Stage { get; init; }
    [Id(3)] public required int Order { get; init; }
    [Id(4)] public required IReadOnlyCollection<GroupStoryPoints> GroupPoints { get; init; }

}

[GenerateSerializer,Immutable]
public class GroupStoryPoints
{
    [Id(0)] public required string Name { get; init; }
    [Id(1)] public required float? StoryPoints { get; init; }
    [Id(2)] public required int? FibonacciStoryPoints { get; init; }
}
[GenerateSerializer,Immutable]
public class GroupedPlayerStateResponse
{
    [Id(0)] public required Guid Id { get; init; }
    [Id(1)] public required string Name { get; init; }
    [Id(2)] public required bool IsCurrentPlayer { get; init; }
    [Id(3)] public required VotingStage? VotingStage { get; init; }
    [Id(4)] public required GroupedPlayerStoryPointState VotingState { get; init; }
}

[GenerateSerializer,Immutable]
public class GroupedPlayerStoryPointState
{
    [Id(0)] public required bool Voted { get; init; }
    [Id(1)] public required int? Value { get; init; }
}

[GenerateSerializer,Immutable]
public class GroupedCurrentPlayerResponse
{
    [Id(0)] public required Guid Id { get; init; }
    [Id(1)] public required bool IsAdded { get; init; }
    [Id(2)] public required bool CanVote { get; init; }
}
[GenerateSerializer,Immutable]
public class GroupedSpectatorStateResponse
{
    [Id(0)]  public required Guid Id { get; init; }
    [Id(1)] public required string Name { get; init; }
    [Id(2)] public required bool IsCurrentPlayer { get; init; }
    [Id(3)] public required bool InRoom { get; init; }
}
