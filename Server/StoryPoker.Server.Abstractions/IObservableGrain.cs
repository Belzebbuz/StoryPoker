namespace StoryPoker.Server.Abstractions;

public interface IObservableGrain<in T>
    where T : IGrainObserver
{
    ValueTask SubscribeAsync(T observer);
    ValueTask UnsubscribeAsync(T observer);
}


