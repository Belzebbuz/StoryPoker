using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoryPoker.Client.Web.Api.Middlewares.CurrentUser;

namespace StoryPoker.Client.Web.Api.Abstractions;

[ApiController]
[Authorize]
public abstract class BaseApiController : ControllerBase
{
    protected IClusterClient GrainClient => HttpContext.RequestServices.GetRequiredService<IClusterClient>();
    protected ICurrentUser CurrentUser =>  HttpContext.RequestServices.GetRequiredService<ICurrentUser>();
}
