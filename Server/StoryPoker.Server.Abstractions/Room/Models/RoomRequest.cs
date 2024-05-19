﻿namespace StoryPoker.Server.Abstractions.Room.Models;

public static class RoomRequest
{
    [GenerateSerializer, Immutable]
    public record CreateRoom(
        [property: Id(0)] Guid PlayerId,
        [property: Id(1)] string PlayerName,
        [property: Id(2)] string RoomName);

    public record AddPlayer([property: Id(0)]Guid Id,[property: Id(1)]string Name);

    public record AddIssue([property: Id(0)]string Title);

    public record SetStoryPoint([property: Id(0)]Guid PlayerId, [property: Id(1)]int StoryPoints);
}

