using StoryPoker.Client.Web.Api.Abstractions;
using StoryPoker.Client.Web.Api.Abstractions.Notifications;
using StoryPoker.Client.Web.Api.Abstractions.Observers;
using StoryPoker.Client.Web.Api.Infrastructure.BackgroundServices.GrainObserver.Channels;
using StoryPoker.Client.Web.Api.Infrastructure.Notifications.Messages;
using StoryPoker.Server.Abstractions.Notifications;

namespace StoryPoker.Client.Web.Api.Infrastructure.BackgroundServices.GrainObserver.Observers;

public class RoomNotificationObserver(
    ILogger<RoomNotificationObserver> logger,
    INotificationService notificationService,
    IGrainSubscriptionBus subscriptionBus,
    IGrainFactory grainFactory)
    : IRoomNotificationObserver, IRoomNotificationGrainSubscriber
{
    private async Task RoomStateChangedAsync(RoomStateChangedNotification notification)
    {
        await notificationService.SendToRoomAsync(notification.RoomId,
            new RoomStateUpdatedMessage(notification.RoomId));
        logger.LogInformation($"Room: {notification.RoomId} state updated");
        if (!notification.PlayerExist)
            await subscriptionBus.EnqueueAsync(new GrainUnsubscription(notification.RoomId));
    }

    public async Task HandleAsync(INotification notification)
    {
        switch (notification)
        {
            case RoomStateChangedNotification roomChanged:
                await RoomStateChangedAsync(roomChanged);
                break;
            default:
                return;
        }
    }

    public async Task StartAsync(Guid notificationGrainId, CancellationToken token)
    {
        var reference = grainFactory.CreateObjectReference<IRoomNotificationObserver>(this);
        IRoomNotificationGrain notificationGrain;
        try
        {
            while (!token.IsCancellationRequested)
            {
                logger.LogInformation($"RoomId: {notificationGrainId} переподписка.");
                notificationGrain = grainFactory.GetGrain<IRoomNotificationGrain>(notificationGrainId);
                await notificationGrain.UnSubscribeAsync(reference);
                await notificationGrain.SubscribeAsync(reference);
                await Task.Delay(TimeSpan.FromMinutes(5), token);
            }
        }
        catch(Exception ex)
        {
            await subscriptionBus.EnqueueAsync(new GrainUnsubscription(notificationGrainId));
        }
        finally
        {
            notificationGrain = grainFactory.GetGrain<IRoomNotificationGrain>(notificationGrainId);
            await notificationGrain.UnSubscribeAsync(reference);
            logger.LogInformation($"RoomId: {notificationGrainId} отписка");
        }
    }
}
