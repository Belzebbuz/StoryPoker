using Newtonsoft.Json;
using StoryPoker.Server.Abstractions.Metadata.Models.Enums;

namespace StoryPoker.Server.Grains.Metadata.Models;

public class InternalRoomMetadata
{
    public required Guid RoomId { get; init; }
    public required Guid OwnerId { get; init; }
    public required RoomType RoomType { get; init; }
    [JsonProperty] public DateTime UpdatedOn { get; private set; }
    public required DateTime CreatedOn { get; init; }
    [JsonProperty] public PaymentLevel PaymentLevel { get; private set; }

    public static InternalRoomMetadata Init(Guid roomId, Guid ownerId, PaymentLevel paymentLevel,
        RoomType roomType)
    {
        return new()
        {
            RoomId = roomId,
            OwnerId = ownerId,
            UpdatedOn = DateTime.UtcNow,
            CreatedOn = DateTime.UtcNow,
            PaymentLevel = paymentLevel,
            RoomType = roomType
        };
    }

    public void Update() => UpdatedOn = DateTime.UtcNow;
}
