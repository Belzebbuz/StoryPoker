using ErrorOr;
using Microsoft.AspNetCore.SignalR;
using StoryPoker.Client.Web.Api.Abstractions.Notifications;
using StoryPoker.Client.Web.Api.Infrastructure.Hubs;
using StoryPoker.Server.Abstractions.Room;

namespace StoryPoker.Client.Web.Api.Infrastructure.Notifications;

internal sealed class NotificationService(IGrainFactory grainFactory, IHubContext<NotificationHub> hub) : INotificationService
{
    private const string ChannelServer = "NotificationServer";
    public async Task<ErrorOr<Success>> SendToRoomAsync<T>(Guid roomId, INotificationMessage<T> message)
    {
        var _ = hub.Clients.Group(roomId.ToString()).SendAsync(ChannelServer, message);
        return Result.Success;
    }
}
