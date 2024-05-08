using Microsoft.AspNetCore.Mvc;
using StoryPoker.Client.Web.Api.Abstractions;
using StoryPoker.Client.Web.Api.Domain.Room.Features.Get;
using StoryPoker.Client.Web.Api.Domain.Room.Features.Init;
using StoryPoker.Server.Abstractions.Room;
using StoryPoker.Server.Abstractions.Room.Models;

namespace StoryPoker.Client.Web.Api.Domain.Room.Controllers;

[Route("api/[controller]")]
public class RoomController : BaseApiController
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetRoomStateResponse>> GetStateAsync(Guid id)
    {
        var grainState = await GrainClient.GetGrain<IRoomGrain>(id).GetAsync();
        if (grainState is null)
            return NotFound();
        var userId = CurrentUser.UserId;
        return Ok(new GetRoomStateResponse(userId, grainState));
    }

    [HttpPost]
    public async Task<ActionResult> InitAsync(InitRoomStateRequest request)
    {
        var roomId = Guid.NewGuid();
        var roomGrain = GrainClient.GetGrain<IRoomGrain>(roomId);
        var userId = CurrentUser.UserId;
        var initStateRequest = request.ToInternal(userId);
        var result = await roomGrain.InitStateAsync(initStateRequest);
        return  result.IsSuccess ? Ok($"/api/room/{roomId}") : BadRequest(result.Error);
    }

    [HttpPost("{id:guid}/players")]
    public async Task<ActionResult> AddPlayerAsync(Guid id, [FromBody]string name)
    {
        var userId = CurrentUser.UserId;
        var request = new AddPlayerRequest(userId, name);
        var result = await GrainClient.GetGrain<IRoomGrain>(id).AddPlayerAsync(request);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
    [HttpPut("{id:guid}/start-vote")]
    public async Task<ActionResult> StartVoteAsync(Guid id)
    {
        var userId = CurrentUser.UserId;
        var result = await GrainClient.GetGrain<IRoomGrain>(id).StartVoteAsync();
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
    [HttpPut("{id:guid}/stop-vote")]
    public async Task<ActionResult> StopVoteAsync(Guid id)
    {
        var userId = CurrentUser.UserId;
        var result = await GrainClient.GetGrain<IRoomGrain>(id).StopVoteAsync();
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
    [HttpPost("{id:guid}/issues")]
    public async Task<ActionResult> AddIssuesAsync(Guid id, [FromBody]string title)
    {
        var request = new AddIssueRequest(title);
        var result = await GrainClient.GetGrain<IRoomGrain>(id).AddIssueAsync(request);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPut("{id:guid}/current-issue")]
    public async Task<ActionResult> SetCurrenIssueAsync(Guid id, [FromQuery] Guid issueId)
    {
        var result = await GrainClient.GetGrain<IRoomGrain>(id).SetCurrentIssueAsync(issueId);
        return result.IsSuccess ? Ok(): BadRequest(result.Error);
    }

    [HttpPut("{id:guid}/issues/current-issue/story-point")]
    public async Task<ActionResult> AddIssuesAsync(Guid id,
        [FromQuery] int storyPoint)
    {
        var userId = CurrentUser.UserId;
        var request = new SetStoryPointRequest(userId, storyPoint);
        var result = await GrainClient.GetGrain<IRoomGrain>(id)
            .SetPlayerIssueStoryPointAsync(request);
        return result.IsSuccess ? Ok(): BadRequest(result.Error);
    }
}
