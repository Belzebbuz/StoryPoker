using Microsoft.Extensions.Logging;
using Orleans.Utilities;
using StoryPoker.Server.Abstractions;

namespace StoryPoker.Server.Grains.Base;

internal abstract class ObservableGrain<T>(ILogger<ObservableGrain<T>> logger) : Grain, IObservableGrain<T>
    where T : IGrainObserver
{
    protected readonly ObserverManager<T> ObserverManager = new(TimeSpan.FromMinutes(60), logger);
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
