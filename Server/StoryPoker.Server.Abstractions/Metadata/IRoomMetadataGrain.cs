using StoryPoker.Server.Abstractions.Metadata.Models;

namespace StoryPoker.Server.Abstractions.Metadata;

public interface IRoomMetadataGrain : IGrainWithGuidKey
{
    Task<ResponseState<Guid>> CreateRoomAsync(CreateRoomRequest request);
    Task<RoomMetadataResponse> GetAsync();
    Task UpdateTimeAsync();
}
