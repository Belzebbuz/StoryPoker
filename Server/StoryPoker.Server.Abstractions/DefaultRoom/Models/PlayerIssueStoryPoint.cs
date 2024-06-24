namespace StoryPoker.Server.Abstractions.DefaultRoom.Models;

[GenerateSerializer, Immutable]
public record PlayerIssueStoryPoint
{
    [Id(0)] public required bool Voted { get; init; }
    [Id(1)] public required int? Value { get; init; }
}