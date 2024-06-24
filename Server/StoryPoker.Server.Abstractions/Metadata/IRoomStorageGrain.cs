namespace StoryPoker.Server.Abstractions.Metadata;

public interface IRoomStorageGrain : IGrainWithGuidKey
{
    Task AddRoomAsync(Guid roomId);
    Task<bool> RoomExistAsync(Guid roomId);
}
