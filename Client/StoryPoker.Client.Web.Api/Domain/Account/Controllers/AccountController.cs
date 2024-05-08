using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace StoryPoker.Client.Web.Api.Domain.Account.Controllers;

[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    [HttpGet("login")]
    public async Task LoginAsync()
    {
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(new ClaimsPrincipal(identity));
    }
}
