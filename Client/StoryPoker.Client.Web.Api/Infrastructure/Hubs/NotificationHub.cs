using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using StoryPoker.Client.Web.Api.Abstractions;
using StoryPoker.Client.Web.Api.Abstractions.Notifications;
using StoryPoker.Client.Web.Api.Infrastructure.BackgroundServices.GrainObserver.Channels;
using StoryPoker.Server.Abstractions.Room;
using StoryPoker.Server.Abstractions.Room.Commands;

namespace StoryPoker.Client.Web.Api.Infrastructure.Hubs;

[Authorize]
public class NotificationHub(
    ILogger<NotificationHub> logger,
    IConnectionStorage connectionStorage,
    IGrainFactory grainFactory,
    IGrainSubscriptionBus subscriptionBus,
    IHostApplicationLifetime lifetime) : Hub
{
    public async Task AddPlayerToRoom(Guid roomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
        if (!Guid.TryParse(Context.UserIdentifier, out var userId))
            throw new ArgumentException(nameof(userId));
        connectionStorage.AddConnection(userId,roomId,Context.ConnectionId);
        await subscriptionBus.EnqueueAsync(new GrainSubscription(roomId));
        logger.LogInformation($"Комната: {roomId} подключился: {Context.ConnectionId}");
    }

    public async Task RemovePlayerFromRoom(Guid roomId)
    {
        await RemovePlayerFromRoomAsync();
        logger.LogInformation($"Комната: {roomId} отключился: {Context.ConnectionId}");
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        logger.LogInformation($"Игрок Id: {Context.UserIdentifier} подключился к {nameof(NotificationHub)}");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
        logger.LogInformation($"Игрок Id:{Context.UserIdentifier} отключился {nameof(NotificationHub)}");
        await Task.Delay(1000);
        await RemovePlayerFromRoomAsync();
    }

    private async Task RemovePlayerFromRoomAsync()
    {
        var roomId = connectionStorage.GetRoomId(Context.ConnectionId);
        if(roomId is null)
            return;
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.Value.ToString());
        connectionStorage.RemoveConnection(Context.ConnectionId);
        if (!Guid.TryParse(Context.UserIdentifier, out var userId))
            throw new ArgumentException(nameof(userId));
        if(lifetime.ApplicationStopping.IsCancellationRequested)
            return;
        if (!connectionStorage.ConnectionExists(userId, roomId.Value))
        {
            var roomGrain = grainFactory.GetGrain<IRoomGrain>(roomId.Value);
            await roomGrain.ExecuteCommandAsync(new RemovePlayerCommand(userId));
            logger.LogInformation($"Игрок Id:{userId} полностью удален из комнаты Id: {roomId}");
        }
    }
}
