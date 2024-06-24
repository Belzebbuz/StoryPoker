using ErrorOr;

namespace StoryPoker.Server.Grains.GroupedRoom.Models;

public record InternalGroup
{
    public required string Name { get; init; }
    public required int Order { get; set; }
    public IReadOnlyDictionary<Guid, InternalGroupedPlayer> Players => _players;
    private readonly Dictionary<Guid, InternalGroupedPlayer> _players = new();

    public ErrorOr<Success> AddPlayer(InternalGroupedPlayer player)
    {
        if (!_players.TryAdd(player.Id, player))
            return Error.Failure(description: "Игрок уже добавлен");
        player.GroupId = Name;
        return Result.Success;
    }

    public ErrorOr<Success> RemovePlayer(Guid userId)
    {
        _players.Remove(userId);
        return Result.Success;
    }
}