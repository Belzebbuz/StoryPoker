using StoryPoker.Client.Web.Api.Middlewares.CurrentUser;

namespace StoryPoker.Client.Web.Api.Abstractions.Forms;

public record DefaultInputForm() : InputBase
{
    public override Task AttachData(IServiceProvider serviceProvider, IDictionary<string, string> parameters) => Task.CompletedTask;
}

public record PlayerNameInputForm() : InputBase<string>
{
    public override Task AttachData(IServiceProvider serviceProvider, IDictionary<string, string> parameters)
    {
        var currentUser = serviceProvider.GetRequiredService<ICurrentUser>();
        Value = currentUser.Name;
        return Task.CompletedTask;
    }
}
public record RoomNameInputForm() : InputBase<string>
{
    public override Task AttachData(IServiceProvider serviceProvider, IDictionary<string, string> parameters)
    {
        Value = $"Планирование {DateTime.Now:g}";
        return Task.CompletedTask;
    }
}
