namespace StoryPoker.Client.Web.Api.Abstractions.Observers;

public interface IRoomNotificationGrainSubscriber
{
    public Task StartAsync(Guid notificationGrainId, CancellationToken token);
}
