using System.Security.Claims;

namespace StoryPoker.Client.Web.Api.Middlewares.CurrentUser;

public interface ICurrentUserInitializer
{
    public void SetCurrentUser(ClaimsPrincipal principal);
}