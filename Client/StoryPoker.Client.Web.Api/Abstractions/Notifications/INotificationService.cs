using ErrorOr;

namespace StoryPoker.Client.Web.Api.Abstractions.Notifications;

public interface INotificationService
{
    Task<ErrorOr<Success>> SendToRoomAsync<T>(Guid roomId, INotificationMessage<T> message);
}

public interface INotificationMessage<out T>
{
    public NotificationMessageType MessageType { get; }
    public T Value { get; }
}

public enum NotificationMessageType : byte
{
    RoomStateUpdated,
    RoomTimerChanged
}
