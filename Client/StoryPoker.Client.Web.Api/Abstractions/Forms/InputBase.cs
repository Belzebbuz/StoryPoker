using System.Text.Json.Serialization;

namespace StoryPoker.Client.Web.Api.Abstractions.Forms;

public abstract record InputBase<T> : InputBase
{
    public T? Value { get; set; }
}
[JsonDerivedType(typeof(UpdateIssueGroupsInputForm))]
[JsonDerivedType(typeof(UpdateGroupedIssueTitleInputForm))]
[JsonDerivedType(typeof(DefaultInputForm))]
[JsonDerivedType(typeof(MultiSelectGroupInputForm))]
[JsonDerivedType(typeof(SelectGroupInputForm))]
[JsonDerivedType(typeof(PlayerNameInputForm))]
[JsonDerivedType(typeof(RoomNameInputForm))]
public abstract record InputBase
{
    public required string Key { get; init; }
    public required string Label { get; init; }
    public required bool Required { get; init; }
    public required byte Order { get; init; }
    public required string ControlType { get; init; }
    public required string Type { get; init; }
    public IDictionary<string, OptionValue>? Options { get; set; }
    public abstract Task AttachData(IServiceProvider serviceProvider, IDictionary<string, string> parameters);
}
public record OptionValue(string Value, ICollection<InputBase> Inputs);
