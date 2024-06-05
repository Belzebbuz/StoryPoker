using ErrorOr;

namespace StoryPoker.Server.Grains.Abstractions;

public interface IDomainEvent
{
    public DateTime CreatedOn { get; }
}
[GenerateSerializer,Immutable]
public abstract record DomainEvent : IDomainEvent
{
    public DateTime CreatedOn { get; } = DateTime.UtcNow;
}
