using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StoryPoker.Server.Abstractions.Room;

namespace StoryPoker.Client.Web.Api.Attributes;

public class RoomExistFilterAttribute: ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ActionArguments.TryGetValue("id", out var idArg))
        {
            await next();
            return;
        }

        if (idArg is not Guid id)
        {
            await next();
            return;
        }
        var grainFactory = context.HttpContext.RequestServices.GetRequiredService<IGrainFactory>();
        var roomStorage = grainFactory.GetGrain<IRoomStorageGrain>(Guid.Empty);
        var roomExist = await roomStorage.RoomExistAsync(id);
        if (roomExist)
            await next();
        else
            context.Result = new NotFoundResult();
    }
}
