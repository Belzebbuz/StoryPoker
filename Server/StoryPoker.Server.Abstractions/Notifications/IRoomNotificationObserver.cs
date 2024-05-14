namespace StoryPoker.Server.Abstractions.Notifications;

public interface IRoomNotificationObserver: IGrainObserver
{
    Task HandleAsync(INotification notification);
}
