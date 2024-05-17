using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StoryPoker.Client.Web.Api.Abstractions;
using StoryPoker.Client.Web.Api.Attributes;
using StoryPoker.Client.Web.Api.Configurations;
using StoryPoker.Client.Web.Api.Domain.Room.Models;
using StoryPoker.Server.Abstractions;
using StoryPoker.Server.Abstractions.Room;
using StoryPoker.Server.Abstractions.Room.Models;
using StoryPoker.Server.Abstractions.Room.Models.Enums;

namespace StoryPoker.Client.Web.Api.Controllers;

[Route("api/[controller]")]
[RoomExistFilter]
public class RoomController : BaseApiController
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RoomStateResponse>> GetStateAsync(Guid id)
    {
        var userId = CurrentUser.UserId;
        var grainState = await GrainClient.GetGrain<IRoomGrain>(id).GetAsync(userId);
        return Ok(grainState);
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync(
        InitRoomStateRequest request,
        [FromServices] IOptionsMonitor<WebHookConfig> webhookConfig)
    {
        var userId = CurrentUser.UserId;
        var initStateRequest = request.ToInternal(userId);
        var result = await GrainClient
            .GetGrain<IRoomStorageGrain>(Guid.Empty)
            .CreateRoomAsync(initStateRequest);
        if (!result.IsSuccess)
            return BadRequest(result.Error);
        var createdRoomLink = string.Format(CultureInfo.InvariantCulture, webhookConfig.CurrentValue.CreatedRoomLink, result.Value);
        return  Ok(new {link = createdRoomLink});
    }

    [HttpPost("{id:guid}/players")]
    public async Task<ActionResult> AddPlayerAsync(
        Guid id,
        AddPlayerRequest request)
    {
        var userId = CurrentUser.UserId;
        var internalRequest = request.ToInternal(userId);
        var result = await GrainClient.GetGrain<IRoomGrain>(id).AddPlayerAsync(internalRequest);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPut("{id:guid}/players/spectator")]
    public async Task<ActionResult> SetNewSpectatorAsync(
        Guid id,
        [FromQuery] Guid playerId)
    {
        var result = await GrainClient.GetGrain<IRoomGrain>(id).SetNewSpectatorAsync(playerId);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpDelete("{id:guid}/players")]
    public async Task<ActionResult> RemovePlayerAsync(Guid id)
    {
        var userId = CurrentUser.UserId;
        var result = await GrainClient.GetGrain<IRoomGrain>(id).RemovePlayerAsync(userId);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPut("{id:guid}/vote-stage")]
    public async Task<ActionResult> StopVoteAsync(Guid id, [FromQuery]VoteStageChangeCommand stage)
    {
        var result = stage switch
        {
            VoteStageChangeCommand.Start => await GrainClient.GetGrain<IRoomGrain>(id).StartVoteAsync(),
            VoteStageChangeCommand.Stop => await GrainClient.GetGrain<IRoomGrain>(id).StopVoteAsync(),
            _ => ResponseState.Fail("Неверная команда")
        };
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPost("{id:guid}/issues")]
    public async Task<ActionResult> AddIssuesAsync(Guid id, AddIssueRequest request)
    {
        var internalRequest = new RoomRequest.AddIssue(request.Title);
        var result = await GrainClient.GetGrain<IRoomGrain>(id).AddIssueAsync(internalRequest);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPut("{id:guid}/issues/order")]
    public async Task<ActionResult> SetIssueOrderbyAsync(Guid id, [FromQuery]IssueOrder order)
    {
        var result = await GrainClient.GetGrain<IRoomGrain>(id).SetIssueListOrderAsync(order);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPut("{id:guid}/issues/{issueId:guid}/order")]
    public async Task<ActionResult> SetIssueNewOrderAsync(Guid id, [FromRoute] Guid issueId, [FromQuery] int newOrder)
    {
        var result = await GrainClient.GetGrain<IRoomGrain>(id).SetIssueOrderAsync(issueId, newOrder);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpDelete("{id:guid}/issues/{issueId:guid}")]
    public async Task<ActionResult> AddIssuesAsync(Guid id,Guid issueId)
    {
        var result = await GrainClient.GetGrain<IRoomGrain>(id).RemoveIssueAsync(issueId);
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
        var request = new RoomRequest.SetStoryPoint(userId, storyPoint);
        var result = await GrainClient.GetGrain<IRoomGrain>(id)
            .SetPlayerIssueStoryPointAsync(request);
        return result.IsSuccess ? Ok(): BadRequest(result.Error);
    }
}
