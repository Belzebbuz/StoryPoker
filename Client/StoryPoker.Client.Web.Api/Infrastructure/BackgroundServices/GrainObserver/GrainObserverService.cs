using System.Collections.Concurrent;
using System.Text.Json;
using StoryPoker.Client.Web.Api.Abstractions;
using StoryPoker.Client.Web.Api.Infrastructure.BackgroundServices.GrainObserver.Channels;
using StoryPoker.Server.Abstractions.Room;

namespace StoryPoker.Client.Web.Api.Infrastructure.BackgroundServices.GrainObserver;

public class GrainObserverService(
    IGrainSubscriptionBus subscriptionBus,
    IServiceScopeFactory scopeFactory,
    IGrainFactory grainFactory,
    ILogger<GrainObserverService> logger) : BackgroundService
{
    private readonly ConcurrentDictionary<GrainSubscription, IGrainObserver> _activeSubscribes = new();
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
        var subscribe = new GrainSubscription(unsubscribe.RoomId);
        if(!_activeSubscribes.TryRemove(subscribe, out var reference))
        {
            logger.LogWarning($"Подписка для комнаты №{unsubscribe.RoomId} не найдена.");
            return;
        }

        if (reference is not IRoomGrainObserver subObj)
        {
            logger.LogWarning($"Подписка для комнаты №{unsubscribe.RoomId} не найдена.");
            return;
        }

        await grainFactory.GetGrain<IRoomGrain>(unsubscribe.RoomId).UnsubscribeAsync(subObj);
        logger.LogInformation($"RoomId: {subscribe.RoomId} подписка удалена");
    }

    private async Task SubscribeGrainAsync(GrainSubscription subscribe)
    {
        try
        {
            await _semaphore.WaitAsync();
            if (_activeSubscribes.ContainsKey(subscribe))
            {
                logger.LogInformation($"RoomId: {subscribe.RoomId} подписка уже создана");
                return;
            }
            await using var scope = scopeFactory.CreateAsyncScope();
            var observer = scope.ServiceProvider.GetRequiredService<IRoomGrainObserver>();
            var reference = grainFactory.CreateObjectReference<IRoomGrainObserver>(observer);
            await grainFactory.GetGrain<IRoomGrain>(subscribe.RoomId).SubscribeAsync(reference);
            _activeSubscribes.TryAdd(subscribe, observer);
            logger.LogInformation($"RoomId: {subscribe.RoomId} подписка создана");
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
