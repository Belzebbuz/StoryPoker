using StoryPoker.Server.Abstractions.Room.Models.Enums;

namespace StoryPoker.Server.Abstractions.Room.Models;

[GenerateSerializer, Immutable]
public record IssueStateResponse
{
    [Id(0)] public required Guid Id { get; init; }
    [Id(1)] public required string Title { get; init; }
    [Id(2)] public required VotingStage Stage { get; init; }
    [Id(3)] public required float? StoryPoints { get; init; }
    [Id(4)] public required int? FibonacciStoryPoints { get; init; }
    [Id(5)] public required int Order { get; init; }
}