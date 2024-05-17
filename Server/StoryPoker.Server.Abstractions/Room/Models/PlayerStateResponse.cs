namespace StoryPoker.Server.Abstractions.Room.Models;

[GenerateSerializer, Immutable]
public record PlayerStateResponse
{
    [Id(0)] public required Guid Id { get; init; }
    [Id(1)] public required string Name { get; init; }
    [Id(2)] public required bool IsSpectator { get; init; }
    [Id(3)] public required PlayerIssueStoryPoint CurrentVotingPoint { get; init; }
}