namespace StoryPoker.Client.Web.Api.Abstractions.Notifications;

public interface IConnectionStorage
{
    public void AddConnection(Guid userId, Guid roomId, string connectionId);
    public void RemoveConnection(string connectionId);

    public Guid? GetRoomId(string connectionId);
    public bool ConnectionExists(Guid userId, Guid roomId);
}
