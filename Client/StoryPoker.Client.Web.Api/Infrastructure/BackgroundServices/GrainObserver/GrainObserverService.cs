using System.Collections.Concurrent;
using StoryPoker.Client.Web.Api.Abstractions;
using StoryPoker.Client.Web.Api.Abstractions.Observers;
using StoryPoker.Client.Web.Api.Infrastructure.BackgroundServices.GrainObserver.Channels;
using StoryPoker.Client.Web.Api.Infrastructure.BackgroundServices.GrainObserver.Observers;
using StoryPoker.Server.Abstractions.Notifications;

namespace StoryPoker.Client.Web.Api.Infrastructure.BackgroundServices.GrainObserver;

public class GrainObserverService(
    IGrainSubscriptionBus subscriptionBus,
    IServiceScopeFactory scopeFactory,
    IGrainFactory grainFactory,
    ILogger<GrainObserverService> logger) : BackgroundService
{
    private readonly ConcurrentDictionary<Guid, (GrainSubscription subscription, IGrainObserver observer)> _activeSubscribes = new();
    private readonly SemaphoreSlim _semaphore = new(1);
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var message in subscriptionBus.ReadAsync(stoppingToken))
        {
            var _ = message switch
            {
                GrainSubscription subscribe => SubscribeGrainAsync(subscribe),
                GrainUnsubscription unsubscribe => UnsubscribeGrainAsync(unsubscribe),
                _ => Task.CompletedTask,
            };
        }
    }

    private async Task UnsubscribeGrainAsync(GrainUnsubscription unsubscribe)
    {
        if(!_activeSubscribes.TryRemove(unsubscribe.RoomId, out var observer))
        {
            logger.LogWarning($"Подписка для комнаты №{unsubscribe.RoomId} не найдена.");
            return;
        }
        await observer.subscription.ResubscribeStoppingToken.CancelAsync();
        logger.LogInformation($"RoomId: {unsubscribe.RoomId} подписка удалена");
    }

    private async Task SubscribeGrainAsync(GrainSubscription subscribe)
    {
        try
        {
            if (_activeSubscribes.ContainsKey(subscribe.RoomId))
            {
                logger.LogInformation($"RoomId: {subscribe.RoomId} подписка уже создана");
                return;
            }
            await _semaphore.WaitAsync();
            if (_activeSubscribes.ContainsKey(subscribe.RoomId))
            {
                logger.LogInformation($"RoomId: {subscribe.RoomId} подписка уже создана");
                return;
            }

            await using var scope = scopeFactory.CreateAsyncScope();
            var subscriber = scope.ServiceProvider.GetRequiredService<IRoomObserverSubscriber>();
            var _ = subscriber.StartAsync(subscribe.RoomId, subscribe.ResubscribeStoppingToken.Token);
            var observer = scope.ServiceProvider.GetRequiredService<IRoomNotificationObserver>();
            _activeSubscribes.TryAdd(subscribe.RoomId, new(subscribe, observer));
            logger.LogInformation($"RoomId: {subscribe.RoomId} подписка создана");
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
