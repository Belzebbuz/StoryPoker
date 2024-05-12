using Microsoft.Extensions.Logging;
using Orleans.Utilities;
using StoryPoker.Server.Abstractions;

namespace StoryPoker.Server.Grains.Base;

internal abstract class ObservableGrain<T>: Grain, IObservableGrain<T>
    where T : IGrainObserver
{
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(1);
    private readonly DateTime _createdTime = DateTime.UtcNow;
    protected readonly ObserverManager<T> ObserverManager;

    protected ObservableGrain( ILogger<ObservableGrain<T>> logger)
    {
        ObserverManager = new(_defaultExpiration, logger)
        {
            GetDateTime = () => _createdTime
        };
    }

    public ValueTask SubscribeAsync(T observer)
    {
        ObserverManager.Subscribe(observer,observer);
        return ValueTask.CompletedTask;
    }

    public ValueTask UnsubscribeAsync(T observer)
    {
        ObserverManager.Unsubscribe(observer);
        return ValueTask.CompletedTask;
    }
}
