using System.Text.Json.Serialization;

namespace StoryPoker.Server.Grains.Abstractions;

public abstract record AggregateRoot
{
    [JsonIgnore]
    private readonly List<IDomainEvent> _events = new();

    protected void AddEvent(IDomainEvent @event)
        => _events.Add(@event);

    public IReadOnlyCollection<IDomainEvent> PopEvents()
    {
        var result = _events.Select(x => x).ToArray().AsReadOnly();
        _events.Clear();
        return result;
    }
}
