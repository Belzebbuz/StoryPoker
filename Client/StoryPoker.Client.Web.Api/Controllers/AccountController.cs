using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace StoryPoker.Client.Web.Api.Controllers;

[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    [HttpGet("login")]
    public async Task LoginAsync()
    {
        if(HttpContext.User.Identity?.IsAuthenticated is true)
            return;
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(new ClaimsPrincipal(identity));
    }
}
