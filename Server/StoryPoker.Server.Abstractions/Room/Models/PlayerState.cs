namespace StoryPoker.Server.Abstractions.Room.Models;

[GenerateSerializer, Immutable]
public class PlayerState
{
    [Id(0)] public Guid Id { get; set; }
    [Id(1)] public required string Name { get; set; }
    [Id(2)] public bool IsSpectator { get; set; }
}
