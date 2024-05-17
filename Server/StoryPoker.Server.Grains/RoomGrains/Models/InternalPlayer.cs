namespace StoryPoker.Server.Grains.RoomGrains.Models;

public record InternalPlayer
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required bool IsSpectator { get; set; }
    public required int Order { get; set; }
}
