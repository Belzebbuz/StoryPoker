using System.Collections.Concurrent;
using StoryPoker.Client.Web.Api.Abstractions.Notifications;

namespace StoryPoker.Client.Web.Api.Infrastructure.Notifications;

internal sealed class ConnectionStorage : IConnectionStorage
{
    private class UserConnections: ConcurrentDictionary<Guid,HashSet<string>>;
    private record UserRoom(Guid UserId, Guid RoomId);

    private readonly ConcurrentDictionary<Guid, UserConnections> _roomConnections = new();
    private readonly ConcurrentDictionary<string, UserRoom> _connectionUserMap = new();
    public void AddConnection(Guid userId, Guid roomId, string connectionId)
    {
        if (!_roomConnections.TryGetValue(roomId, out var users))
        {
            users = new();
            _roomConnections[roomId] = users;
        }
        if(!users.TryGetValue(userId, out var connections))
        {
            connections = new();
            users[userId] = connections;
        };
        connections.Add(connectionId);
        _connectionUserMap[connectionId] = new(userId,roomId);
    }

    public void RemoveConnection(string connectionId)
    {
        if (_connectionUserMap.TryRemove(connectionId, out var userRoom))
        {
            _roomConnections[userRoom.RoomId][userRoom.UserId].Remove(connectionId);
        }
    }

    public Guid? GetRoomId(string connectionId) => _connectionUserMap.GetValueOrDefault(connectionId)?.RoomId;

    public bool ConnectionExists(Guid userId, Guid roomId) => _roomConnections[roomId][userId].Count != 0;
}
