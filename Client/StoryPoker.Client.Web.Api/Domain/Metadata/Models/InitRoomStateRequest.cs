using System.Text.Json.Serialization;
using StoryPoker.Client.Web.Api.Domain.Common;
using StoryPoker.Server.Abstractions.Metadata.Models;
using StoryPoker.Server.Abstractions.Metadata.Models.Enums;

namespace StoryPoker.Client.Web.Api.Domain.Metadata.Models;

public record InitRoomStateRequest(
    string RoomName,
    string PlayerName,
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    RoomType RoomType,
    ICollection<string> GroupNames) : IPlayerNameRequest
{
    public CreateRoomRequest ToInternal(Guid playerId)
        => new(playerId, PlayerName, RoomName, RoomType,GroupNames);
}
