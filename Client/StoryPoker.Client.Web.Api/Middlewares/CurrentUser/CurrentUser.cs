using System.Security.Claims;

namespace StoryPoker.Client.Web.Api.Middlewares.CurrentUser;

internal class CurrentUser : ICurrentUser, ICurrentUserInitializer
{
    private ClaimsPrincipal? _user;
    private readonly Guid _userId = Guid.Empty;
    public Guid UserId  => _user?.Identity?.IsAuthenticated is true
        ? Guid.Parse(_user?.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString())
        : _userId;

    public void SetCurrentUser(ClaimsPrincipal principal)
    {
        _user = principal;
    }
}