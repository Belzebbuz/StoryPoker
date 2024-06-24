using StoryPoker.Server.Grains.Abstractions;

namespace StoryPoker.Server.Grains.GroupedRoom.Models.DomainEvents;
[GenerateSerializer, Immutable]
public record GroupedVoteEndingTimerEvent([property: Id(0)]Guid IssueId) : DomainEvent;
