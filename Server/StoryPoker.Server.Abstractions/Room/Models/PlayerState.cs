namespace StoryPoker.Server.Abstractions.Room.Models;

[GenerateSerializer, Immutable]
public class PlayerState
{
    [Id(0)] public required Guid Id { get; set; }
    [Id(1)] public required string Name { get; set; }
    [Id(2)] public required bool IsSpectator { get; set; }
    [Id(3)] public required int Order { get; set; }
}
