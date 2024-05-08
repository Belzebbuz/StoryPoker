namespace StoryPoker.Server.Abstractions.Room.Models;

[GenerateSerializer, Immutable]
public record InitStateRequest(
    [property: Id(0)] Guid PlayerId,
    [property: Id(0)] string PlayerName,
    [property: Id(1)] string RoomName);

[GenerateSerializer, Immutable]
public record AddPlayerRequest(
    [property: Id(0)] Guid Id,
    [property: Id(1)] string Name);

[GenerateSerializer, Immutable]
public record AddIssueRequest(
    [property: Id(0)] string Title);

[GenerateSerializer, Immutable]
public record SetStoryPointRequest(
    [property: Id(0)] Guid PlayerId,
    [property: Id(1)] int StoryPoints);
