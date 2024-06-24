namespace StoryPoker.Server.Abstractions.DefaultRoom.Models;

public static class DefaultRoomRequest
{
    public record AddPlayer([property: Id(0)]Guid Id,[property: Id(1)]string Name);

    public record AddIssue([property: Id(0)]string Title);

    public record SetStoryPoint([property: Id(0)]Guid PlayerId, [property: Id(1)]int StoryPoints);
}

