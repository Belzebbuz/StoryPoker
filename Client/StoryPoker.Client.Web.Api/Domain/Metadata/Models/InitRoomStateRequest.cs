using System.Text.Json.Serialization;
using StoryPoker.Server.Abstractions.Metadata.Models;
using StoryPoker.Server.Abstractions.Metadata.Models.Enums;

namespace StoryPoker.Client.Web.Api.Domain.Room.Models;

public record InitRoomStateRequest(
    string RoomName,
    string PlayerName,
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    RoomType RoomType,
    ICollection<string> GroupNames)
{
    public CreateRoomRequest ToInternal(Guid playerId)
        => new(playerId, PlayerName, RoomName, RoomType, GroupNames);
}
