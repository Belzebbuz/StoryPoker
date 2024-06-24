namespace StoryPoker.Server.Abstractions.GroupedRoom.Models;

[GenerateSerializer,Immutable]
public record GetIssueResponse([property: Id(0)]string Title, [property: Id(1)]string[] GroupNames);
