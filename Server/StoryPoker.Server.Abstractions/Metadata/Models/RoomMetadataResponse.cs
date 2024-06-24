using StoryPoker.Server.Abstractions.Metadata.Models.Enums;

namespace StoryPoker.Server.Abstractions.Metadata.Models;

[GenerateSerializer, Immutable]
public record RoomMetadataResponse()
{
    [Id(0)]public required DateTime CreatedOn { get; init; }
    [Id(1)]public required DateTime UpdateOn { get; init; }
    [Id(2)]public required RoomType RoomType { get; init; }
    [Id(3)]public required Guid OwnerId { get; init; }
    [Id(4)]public required PaymentLevel PaymentLevel { get; init; }
}
