using Microsoft.AspNetCore.Mvc;
using StoryPoker.Client.Web.Api.Abstractions;
using StoryPoker.Client.Web.Api.Attributes;
using StoryPoker.Client.Web.Api.Domain.Common;
using StoryPoker.Server.Abstractions.DefaultRoom;
using StoryPoker.Server.Abstractions.DefaultRoom.Commands;
using StoryPoker.Server.Abstractions.DefaultRoom.Models;

namespace StoryPoker.Client.Web.Api.Controllers;

[Route("api/room/d")]
[RoomExistFilter]
public class DefaultRoomController : BaseApiController
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DefaultRoomStateResponse>> GetStateAsync(Guid id)
    {
        var userId = CurrentUser.UserId;
        var grainState = await GetGrain(id).GetAsync(userId);
        return Ok(grainState);
    }

    [HttpPost("{id:guid}/command")]
    public async Task<ActionResult> ExecuteCommandAsync(Guid id, DefaultRoomCommandRequest request)
    {
        var userId = CurrentUser.UserId;
        var internalCommand = request.ToInternalCommand(userId);
        var result = await GetGrain(id)
            .ExecuteCommandAsync(internalCommand);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    private IDefaultRoomGrain GetGrain(Guid id)
    {
        return GrainClient.GetGrain<IDefaultRoomGrain>(id);
    }
}
