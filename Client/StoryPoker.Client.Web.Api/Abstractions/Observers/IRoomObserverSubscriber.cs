namespace StoryPoker.Client.Web.Api.Abstractions.Observers;

public interface IRoomObserverSubscriber
{
    public Task StartAsync(Guid notificationGrainId, CancellationToken token);
}
