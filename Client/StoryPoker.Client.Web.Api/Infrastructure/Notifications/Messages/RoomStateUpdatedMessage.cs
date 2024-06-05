using StoryPoker.Client.Web.Api.Abstractions.Notifications;

namespace StoryPoker.Client.Web.Api.Infrastructure.Notifications.Messages;

public record RoomStateUpdatedMessage(Guid Value) : INotificationMessage<Guid>
{
    public NotificationMessageType MessageType => NotificationMessageType.RoomStateUpdated;
}

public record RoomVoteEndingTimerMessage(short Value) : INotificationMessage<short>
{
    public NotificationMessageType MessageType => NotificationMessageType.RoomTimerChanged;
}
