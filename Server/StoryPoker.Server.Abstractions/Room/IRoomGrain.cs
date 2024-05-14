using StoryPoker.Server.Abstractions.Room.Models;

namespace StoryPoker.Server.Abstractions.Room;

public interface IRoomGrain : IGrainWithGuidKey
{
    Task<RoomGrainState> GetAsync();
    Task<ResponseState> InitStateAsync(RoomRequest.CreateRoom request);
    Task<ResponseState> AddPlayerAsync(RoomRequest.AddPlayer request);
    Task<ResponseState> RemovePlayerAsync(Guid playerId);
    Task<ResponseState> AddIssueAsync(RoomRequest.AddIssue request);
    Task<ResponseState> RemoveIssueAsync(Guid issueId);
    Task<ResponseState> SetCurrentIssueAsync(Guid issueId);
    Task<ResponseState> SetPlayerIssueStoryPointAsync(RoomRequest.SetStoryPoint request);
    Task<ResponseState> StartVoteAsync();
    Task<ResponseState> StopVoteAsync();
}
