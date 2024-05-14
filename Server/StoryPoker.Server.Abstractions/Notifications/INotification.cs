namespace StoryPoker.Server.Abstractions.Notifications;

public interface INotification;

[GenerateSerializer, Immutable]
public record RoomStateChangedNotification([property: Id(0)]Guid RoomId, bool playerExist) : INotification;
