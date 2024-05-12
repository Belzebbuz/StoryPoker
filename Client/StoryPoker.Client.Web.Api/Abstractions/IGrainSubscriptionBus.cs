namespace StoryPoker.Client.Web.Api.Abstractions;

public interface IGrainSubscriptionBus
{
    public Task EnqueueAsync(IGrainSubscriptionMessage message);
    public IAsyncEnumerable<IGrainSubscriptionMessage> ReadAsync(CancellationToken token);
}

public interface IGrainSubscriptionMessage;
