namespace StoryPoker.Server.Grains.GroupedRoom.Models;

public record InternalGroupedSpectator
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public bool InRoom { get; set; }
};