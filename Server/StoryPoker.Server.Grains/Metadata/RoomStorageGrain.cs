using Orleans.Core;
using Orleans.Runtime;
using StoryPoker.Server.Abstractions.Metadata;
using StoryPoker.Server.Grains.Constants;

namespace StoryPoker.Server.Grains.Metadata;

public class RoomStorageGrain(
    [PersistentState(stateName: "room-storage-state", storageName: StorageConstants.PersistenceStorage)]
    IStorage<HashSet<Guid>> _rooms) : Grain, IRoomStorageGrain
{
    public async Task AddRoomAsync(Guid roomId)
    {
        _rooms.State.Add(roomId);
        await _rooms.WriteStateAsync();
    }

    public Task<bool> RoomExistAsync(Guid roomId) => Task.FromResult(_rooms.State.Contains(roomId));
}
