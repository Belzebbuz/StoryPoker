namespace StoryPoker.Server.Abstractions.Notifications;

public interface INotification;

[GenerateSerializer, Immutable]
public record RoomStateChangedNotification(
    [property: Id(0)]Guid RoomId,
    [property: Id(1)]bool PlayerExist) : INotification;

[GenerateSerializer, Immutable]
public record RoomVoteEndingTimerNotification(
    [property: Id(0)]Guid RoomId,
    [property: Id(1)]short TimeLeft) : INotification;
