using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StoryPoker.Client.Web.Api.Abstractions;
using StoryPoker.Client.Web.Api.Configurations;
using StoryPoker.Client.Web.Api.Domain.Room.Models;
using StoryPoker.Server.Abstractions.Metadata;
using StoryPoker.Server.Abstractions.Metadata.Models.Enums;

namespace StoryPoker.Client.Web.Api.Controllers;

[Route("api/room/metadata")]
public class RoomMetadataController : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult> CreateAsync(
        InitRoomStateRequest request,
        [FromServices] IOptionsMonitor<WebHookConfig> webhookConfig)
    {
        var userId = CurrentUser.UserId;
        var initStateRequest = request.ToInternal(userId);
        var result = await GrainClient
            .GetGrain<IRoomMetadataGrain>(Guid.NewGuid())
            .CreateRoomAsync(initStateRequest);
        if (!result.IsSuccess)
            return BadRequest(result.Error);
        var uri = request.RoomType switch
        {
            RoomType.Default => webhookConfig.CurrentValue.DefaultRoomLink,
            RoomType.Grouped => webhookConfig.CurrentValue.GroupedRoomLink,
            _ => throw new NotImplementedException()
        };
        var createdRoomLink = string.Format(CultureInfo.InvariantCulture, uri, result.Value);
        return  Ok(new {link = createdRoomLink});
    }
}
