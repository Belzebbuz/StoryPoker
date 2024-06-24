using ErrorOr;

namespace StoryPoker.Client.Web.Api.Abstractions.Forms;

public interface IFormService
{
    public Task<ErrorOr<IEnumerable<InputBase>>> GetFormAsync(string key, IDictionary<string, string> parameters);
}
