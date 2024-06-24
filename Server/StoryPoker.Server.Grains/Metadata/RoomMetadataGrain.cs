using Orleans.Core;
using Orleans.Runtime;
using StoryPoker.Server.Abstractions;
using StoryPoker.Server.Abstractions.Common;
using StoryPoker.Server.Abstractions.DefaultRoom;
using StoryPoker.Server.Abstractions.DefaultRoom.Commands;
using StoryPoker.Server.Abstractions.DefaultRoom.Models;
using StoryPoker.Server.Abstractions.GroupedRoom;
using StoryPoker.Server.Abstractions.Metadata;
using StoryPoker.Server.Abstractions.Metadata.Models;
using StoryPoker.Server.Abstractions.Metadata.Models.Enums;
using StoryPoker.Server.Grains.Constants;
using StoryPoker.Server.Grains.Metadata.Models;

namespace StoryPoker.Server.Grains.Metadata;

public class RoomMetadataGrain(
    [PersistentState(stateName: "room-metadata-state", storageName: StorageConstants.PersistenceStorage)]
    IStorage<InternalRoomMetadata> _metadata) :Grain, IRoomMetadataGrain
{
    public async Task<ResponseState<Guid>> CreateRoomAsync(CreateRoomRequest request)
    {
        var roomId = this.GetPrimaryKey();
        var roomGrain = GetGrain(request.RoomType, roomId);
        var result = await roomGrain.InitStateAsync(request);
        if (!result.IsSuccess)
            return ResponseState<Guid>.Fail(result.Error!);
        var metadata = InternalRoomMetadata.Init(roomId, request.PlayerId, PaymentLevel.MaxLevel, request.RoomType);
        _metadata.State = metadata;
        await _metadata.WriteStateAsync();

        await GrainFactory
            .GetGrain<IRoomStorageGrain>(Guid.Empty)
            .AddRoomAsync(roomId);

        return ResponseState<Guid>.Success(roomId);
    }

    public Task<RoomMetadataResponse> GetAsync()
    {
        var response = new RoomMetadataResponse
        {
            CreatedOn = _metadata.State.CreatedOn,
            UpdateOn =  _metadata.State.UpdatedOn,
            RoomType = _metadata.State.RoomType,
            OwnerId = _metadata.State.OwnerId,
            PaymentLevel = _metadata.State.PaymentLevel
        };
        return Task.FromResult(response);
    }

    private IRoomGrain GetGrain(RoomType requestRoomType, Guid roomId) =>
        requestRoomType switch
        {
            RoomType.Default => GrainFactory.GetGrain<IDefaultRoomGrain>(roomId),
            RoomType.Grouped => GrainFactory.GetGrain<IGroupedRoomGrain>(roomId),
            _ => throw new ArgumentOutOfRangeException(nameof(requestRoomType), requestRoomType, null)
        };

    public async Task UpdateTimeAsync()
    {
        _metadata.State.Update();
        await _metadata.WriteStateAsync();
    }
}
