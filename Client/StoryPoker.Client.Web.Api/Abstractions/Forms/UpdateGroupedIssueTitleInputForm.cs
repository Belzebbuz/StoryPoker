using StoryPoker.Server.Abstractions.GroupedRoom;

namespace StoryPoker.Client.Web.Api.Abstractions.Forms;

public record UpdateGroupedIssueTitleInputForm() : InputBase<string>
{
    private const string RoomIdParameterName = "roomId";
    private const string IssueIdParameterName = "issueId";
    public override async Task AttachData(IServiceProvider serviceProvider, IDictionary<string, string> parameters)
    {
        if (!parameters.TryGetValue(RoomIdParameterName, out var stringRoomId))
            throw new ArgumentException(RoomIdParameterName);
        if(!Guid.TryParse(stringRoomId, out var roomId))
            throw new ArgumentException(RoomIdParameterName);

        if (!parameters.TryGetValue(IssueIdParameterName, out var stringIssueId))
            throw new ArgumentException(IssueIdParameterName);
        if(!Guid.TryParse(stringIssueId, out var issueId))
            throw new ArgumentException(IssueIdParameterName);
        var grainFactory = serviceProvider.GetRequiredService<IGrainFactory>();
        var issue = await grainFactory.GetGrain<IGroupedRoomGrain>(roomId).GetIssueAsync(issueId);
        Value = issue.Title;
    }
}
