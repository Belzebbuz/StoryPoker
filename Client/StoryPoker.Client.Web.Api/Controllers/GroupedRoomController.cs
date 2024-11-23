using Microsoft.AspNetCore.Mvc;
using StoryPoker.Client.Web.Api.Abstractions;
using StoryPoker.Client.Web.Api.Attributes;
using StoryPoker.Client.Web.Api.Domain.Common;
using StoryPoker.Server.Abstractions.Common;
using StoryPoker.Server.Abstractions.GroupedRoom;
using StoryPoker.Server.Abstractions.GroupedRoom.Commands;
using StoryPoker.Server.Abstractions.GroupedRoom.Models;

namespace StoryPoker.Client.Web.Api.Controllers;

[Route("api/room/g")]
[RoomExistFilter]
public class GroupedRoomController : BaseApiController
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GroupedRoomResponse>> GetStateAsync(Guid id)
    {
        var userId = CurrentUser.UserId;
        var grainState = await GetGrain(id).GetAsync(userId);
        return Ok(grainState);
    }
    [HttpPost("{id:guid}/command")]
    public async Task<ActionResult> ExecuteCommandAsync(Guid id, GroupedRoomCommandRequest request)
    {
        if (request is AddPlayerGroupedRoomRequest req)
            await SaveNameToCookieAsync(req);
        var userId = CurrentUser.UserId;
        var internalCommand = request.ToInternalCommand(userId);
        var result = await GetGrain(id)
            .ExecuteCommandAsync(internalCommand);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
    private IGroupedRoomGrain GetGrain(Guid id)
        => GrainClient.GetGrain<IGroupedRoomGrain>(id);
}
