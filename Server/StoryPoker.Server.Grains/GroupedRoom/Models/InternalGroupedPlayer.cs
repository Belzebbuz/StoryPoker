namespace StoryPoker.Server.Grains.GroupedRoom.Models;

public record InternalGroupedPlayer
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string GroupId { get; set; }
}