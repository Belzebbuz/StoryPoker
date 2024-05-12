using Orleans.Core;
using Orleans.Runtime;
using StoryPoker.Server.Abstractions;
using StoryPoker.Server.Abstractions.Room;
using StoryPoker.Server.Abstractions.Room.Models;
using StoryPoker.Server.Grains.Constants;

namespace StoryPoker.Server.Grains.RoomGrains;

public class RoomStorageGrain(
    [PersistentState(stateName: "room-storage-state", storageName: StorageConstants.PersistenceStorage)]
    IStorage<HashSet<Guid>> _rooms) :Grain, IRoomStorageGrain
{
    public async Task<ResponseState<Guid>> CreateRoomAsync(RoomRequest.CreateRoom request)
    {
        var roomId = Guid.NewGuid();
        var roomGrain = GrainFactory.GetGrain<IRoomGrain>(roomId);
        var result = await roomGrain.InitStateAsync(request);
        if (!result.IsSuccess)
            return ResponseState<Guid>.Fail(result.Error!);
        _rooms.State.Add(roomId);
        await _rooms.WriteStateAsync();
        return ResponseState<Guid>.Success(roomId);
    }

    public Task<bool> RoomExistAsync(Guid roomId) => Task.FromResult( _rooms.State.Contains(roomId));
}
