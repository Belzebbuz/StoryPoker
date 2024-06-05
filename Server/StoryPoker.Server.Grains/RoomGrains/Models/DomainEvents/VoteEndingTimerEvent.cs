using StoryPoker.Server.Grains.Abstractions;

namespace StoryPoker.Server.Grains.RoomGrains.Models.DomainEvents;

[GenerateSerializer, Immutable]
public record VoteEndingTimerEvent([property: Id(0)]Guid IssueId) : DomainEvent;
