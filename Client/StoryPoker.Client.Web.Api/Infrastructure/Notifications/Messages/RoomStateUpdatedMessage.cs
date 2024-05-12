using StoryPoker.Client.Web.Api.Abstractions.Notifications;

namespace StoryPoker.Client.Web.Api.Infrastructure.Notifications.Messages;

public record RoomStateUpdatedMessage(Guid Value) : INotificationMessage<Guid>
{
    public NotificationMessageType MessageType => NotificationMessageType.RoomStateUpdated;
}
