using StoryPoker.Server.Abstractions.Common;
using StoryPoker.Server.Abstractions.GroupedRoom.Commands;
using StoryPoker.Server.Abstractions.GroupedRoom.Models;

namespace StoryPoker.Server.Abstractions.GroupedRoom;

public interface IGroupedRoomGrain :  IRoomGrain<GroupedRoomCommand,GroupedRoomResponse>
{
    Task<GetGroupsResponse> GetGroupsAsync();
    Task<GetIssueResponse> GetIssueAsync(Guid issueId);
}
