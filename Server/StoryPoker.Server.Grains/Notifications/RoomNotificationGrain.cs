using Microsoft.Extensions.Logging;
using Orleans.Concurrency;
using Orleans.Utilities;
using StoryPoker.Server.Abstractions.Notifications;

namespace StoryPoker.Server.Grains.Notifications;

[StatelessWorker]
public class RoomNotificationGrain(ILogger<RoomNotificationGrain> logger) : Grain, IRoomNotificationGrain
{
    private readonly ObserverManager<IRoomNotificationObserver> _observerManager = new(TimeSpan.FromMinutes(60), logger);

    public async Task NotifyAsync(INotification notification)
    {
        await _observerManager.Notify(s => s.HandleAsync(notification));
    }

    public Task SubscribeAsync(IRoomNotificationObserver observer)
    {
        _observerManager.Subscribe(observer, observer);
        return Task.CompletedTask;
    }

    public Task UnSubscribeAsync(IRoomNotificationObserver observer)
    {
        _observerManager.Unsubscribe(observer);
        return Task.CompletedTask;
    }
}
