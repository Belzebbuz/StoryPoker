namespace StoryPoker.Server.Abstractions.Room;

public interface IRoomGrainObserver : IGrainObserver
{
    Task RoomStateChangedAsync();
}
