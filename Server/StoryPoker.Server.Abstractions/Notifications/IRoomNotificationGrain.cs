namespace StoryPoker.Server.Abstractions.Notifications;

public interface IRoomNotificationGrain : IGrainWithGuidKey
{
    Task NotifyAsync(INotification notification);
    Task SubscribeAsync(IRoomNotificationObserver observer);

    Task UnSubscribeAsync(IRoomNotificationObserver observer);
}
