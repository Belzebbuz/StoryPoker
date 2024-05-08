using StoryPoker.Server.Abstractions.Room.Models;

namespace StoryPoker.Server.Abstractions.Room;

public interface IRoomGrain : IGrainWithGuidKey, IObservableGrain<IRoomGrainObserver>
{
    Task<RoomGrainState?> GetAsync();
    Task<ResponseState> InitStateAsync(InitStateRequest request);
    Task<ResponseState> AddPlayerAsync(AddPlayerRequest request);
    Task<ResponseState> AddIssueAsync(AddIssueRequest request);
    Task<ResponseState> RemoveIssueAsync(Guid issueId);
    Task<ResponseState> SetCurrentIssueAsync(Guid issueId);
    Task<ResponseState> SetPlayerIssueStoryPointAsync(SetStoryPointRequest request);
    Task<ResponseState> StartVoteAsync();
    Task<ResponseState> StopVoteAsync();
}
