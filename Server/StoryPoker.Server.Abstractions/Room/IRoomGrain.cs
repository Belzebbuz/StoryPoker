using StoryPoker.Server.Abstractions.Room.Models;
using StoryPoker.Server.Abstractions.Room.Models.Enums;

namespace StoryPoker.Server.Abstractions.Room;

public interface IRoomGrain : IGrainWithGuidKey
{
    Task<RoomStateResponse> GetAsync(Guid playerId);
    Task<ResponseState> InitStateAsync(RoomRequest.CreateRoom request);
    Task<ResponseState> AddPlayerAsync(RoomRequest.AddPlayer request);
    Task<ResponseState> RemovePlayerAsync(Guid playerId);
    Task<ResponseState> AddIssueAsync(RoomRequest.AddIssue request);
    Task<ResponseState> RemoveIssueAsync(Guid issueId);
    Task<ResponseState> SetCurrentIssueAsync(Guid issueId);
    Task<ResponseState> SetPlayerIssueStoryPointAsync(RoomRequest.SetStoryPoint request);
    Task<ResponseState> StartVoteAsync();
    Task<ResponseState> StopVoteAsync();
    Task<ResponseState> SetNewSpectatorAsync(Guid playerId);
    Task<ResponseState> SetIssueListOrderAsync(IssueOrder order);
    Task<ResponseState> SetIssueOrderAsync(Guid issueId, int newOrder);
}
