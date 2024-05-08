namespace StoryPoker.Server.Abstractions.Room.Models;

[GenerateSerializer, Immutable]
public class IssueState
{
    [Id(0)] public Guid Id { get; init; }
    [Id(1)] public required string Title { get; init; }
    [Id(2)] public int? StoryPoints { get; set; }
    [Id(3)] public Dictionary<Guid, int> PlayerStoryPoints { get; set; } = [];
}
