using StoryPoker.Client.Web.Api.Abstractions;
using StoryPoker.Client.Web.Api.Abstractions.Notifications;
using StoryPoker.Client.Web.Api.Infrastructure.BackgroundServices.GrainObserver.Channels;
using StoryPoker.Client.Web.Api.Infrastructure.Notifications.Messages;
using StoryPoker.Server.Abstractions.Room;

namespace StoryPoker.Client.Web.Api.Infrastructure.BackgroundServices.GrainObserver.Observers;

public class RoomObserver(
    ILogger<RoomObserver> logger,
    INotificationService notificationService,
    IGrainSubscriptionBus subscriptionBus) : IRoomGrainObserver
{
    public async Task RoomStateChangedAsync(Guid roomId, bool playersExist)
    {
       await notificationService.SendToRoomAsync(roomId, new RoomStateUpdatedMessage(roomId));
       logger.LogInformation($"Room: {roomId} state updated");
       if (!playersExist)
           await subscriptionBus.EnqueueAsync(new GrainUnsubscription(roomId));
    }
}
