using StoryPoker.Server.Abstractions.DefaultRoom.Models.Enums;
using StoryPoker.Server.Abstractions.Metadata.Models.Enums;

namespace StoryPoker.Server.Abstractions.Metadata.Models;

[GenerateSerializer, Immutable]
public record CreateRoomRequest(
    [property: Id(0)] Guid PlayerId,
    [property: Id(1)] string PlayerName,
    [property: Id(2)] string RoomName,
    [property: Id(3)] RoomType RoomType,
    [property: Id(4)] ICollection<string> GroupNames);
