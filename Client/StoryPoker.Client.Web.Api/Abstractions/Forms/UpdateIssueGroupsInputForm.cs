using StoryPoker.Server.Abstractions.GroupedRoom;

namespace StoryPoker.Client.Web.Api.Abstractions.Forms;

public record UpdateIssueGroupsInputForm() : InputBase<IEnumerable<string>>
{
    private const string RoomIdParameterName = "roomId";
    private const string IssueIdParameterName = "issueId";
    public override async Task AttachData(IGrainFactory grainFactory, IDictionary<string, string> parameters)
    {
        if (!parameters.TryGetValue(RoomIdParameterName, out var stringRoomId))
            throw new ArgumentException(RoomIdParameterName);
        if(!Guid.TryParse(stringRoomId, out var roomId))
            throw new ArgumentException(RoomIdParameterName);

        if (!parameters.TryGetValue(IssueIdParameterName, out var stringIssueId))
            throw new ArgumentException(IssueIdParameterName);
        if(!Guid.TryParse(stringIssueId, out var issueId))
            throw new ArgumentException(IssueIdParameterName);
        var issue = await grainFactory.GetGrain<IGroupedRoomGrain>(roomId).GetIssueAsync(issueId);
        var allGroupNames = await grainFactory.GetGrain<IGroupedRoomGrain>(roomId).GetGroupsAsync();
        Value = issue.GroupNames;
        Options = allGroupNames.Groups.ToDictionary(x => x.Key.ToString(), x => new OptionValue(x.Value, []));
    }
}