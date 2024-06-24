using Microsoft.AspNetCore.Mvc;
using StoryPoker.Client.Web.Api.Abstractions;
using StoryPoker.Client.Web.Api.Abstractions.Forms;

namespace StoryPoker.Client.Web.Api.Controllers;

[Route("api/[controller]")]
public class FormConfigController(IFormService formService) : BaseApiController
{
    [HttpPost("{key}")]
    public async Task<ActionResult> GetFormConfig(string key, IDictionary<string, string> parameters)
    {
        var result = await formService.GetFormAsync(key, parameters);
        return result.Match(Ok, Problem);
    }
}
