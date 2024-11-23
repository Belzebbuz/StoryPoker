using System.Security.Claims;
using ErrorOr;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using StoryPoker.Client.Web.Api.Domain.Common;
using StoryPoker.Client.Web.Api.Middlewares.CurrentUser;
using Throw;

namespace StoryPoker.Client.Web.Api.Abstractions;

[ApiController]
[Authorize]
public abstract class BaseApiController : ControllerBase
{
    protected IClusterClient GrainClient => HttpContext.RequestServices.GetRequiredService<IClusterClient>();
    protected ICurrentUser CurrentUser =>  HttpContext.RequestServices.GetRequiredService<ICurrentUser>();
    protected ActionResult Problem(List<Error> errors)
    {
        if (!errors.Any())
        {
            return Problem();
        }
        if (errors.All(error => error.Type == ErrorType.Validation))
        {
            return ValidationProblem(errors);
        }

        return Problem(errors[0]);
    }
    private ObjectResult Problem(Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Unauthorized => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError,
        };

        return Problem(statusCode: statusCode, title: error.Description);
    }
    private ActionResult ValidationProblem(List<Error> errors)
    {
        var modelStateDictionary = new ModelStateDictionary();

        errors.ForEach(error => modelStateDictionary.AddModelError(error.Code, error.Description));

        return ValidationProblem(modelStateDictionary);
    }

    protected async Task SaveNameToCookieAsync(IPlayerNameRequest request)
    {
        if(!string.IsNullOrEmpty(CurrentUser.Name) && CurrentUser.Name == request.PlayerName)
            return;
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, CurrentUser.UserId.ThrowIfNull().Value.ToString()),
            new(ClaimTypes.Name, request.PlayerName)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(new ClaimsPrincipal(identity));
    }
}
