
namespace StoryPoker.Server.Grains.Abstractions;

public interface IRoomEventRunnerGrain: IGrainWithGuidKey
{
    public Task RunAsync(IDomainEvent @event);
}

public interface IRoomEventRunnerGrain<in T> : IRoomEventRunnerGrain where T : IDomainEvent
{
    public Task RunAsync(T @event);
}
