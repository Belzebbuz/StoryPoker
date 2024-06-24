namespace StoryPoker.Server.Abstractions.GroupedRoom.Models;

[GenerateSerializer,Immutable]
public record GetGroupsResponse([property: Id(0)]IDictionary<string,string> Groups);
