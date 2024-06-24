namespace StoryPoker.Client.Web.Api.Abstractions.Forms;

public record DefaultInputForm() : InputBase
{
    public override Task AttachData(IGrainFactory grainFactory, IDictionary<string, string> parameters) => Task.CompletedTask;
}
