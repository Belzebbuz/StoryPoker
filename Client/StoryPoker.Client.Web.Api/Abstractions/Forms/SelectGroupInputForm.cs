using StoryPoker.Server.Abstractions.GroupedRoom;

namespace StoryPoker.Client.Web.Api.Abstractions.Forms;

public record SelectGroupInputForm() : InputBase
{
    private const string RoomIdParameterName = "roomId";
    public override async Task AttachData(IGrainFactory grainFactory, IDictionary<string, string> parameters)
    {
        if (!parameters.TryGetValue(RoomIdParameterName, out var stringRoomId))
            throw new ArgumentException(nameof(stringRoomId));
        if(!Guid.TryParse(stringRoomId, out var roomId))
            throw new ArgumentException(nameof(stringRoomId));
        var state = await grainFactory.GetGrain<IGroupedRoomGrain>(roomId).GetGroupsAsync();
        Options = state.Groups.ToDictionary(x => x.Key.ToString(), x => new OptionValue(x.Value, []));
    }
}