using StoryPoker.Server.Abstractions.Common;
using StoryPoker.Server.Abstractions.DefaultRoom.Models.Enums;

namespace StoryPoker.Server.Abstractions.DefaultRoom.Models;

[GenerateSerializer, Immutable]
public record DefaultRoomStateResponse : IRoomStateResponse
{
    [Id(0)]public required Guid PlayerId { get; init; }
    [Id(1)]public required bool IsPlayerAdded { get; init;  }
    [Id(2)]public required bool IsSpectator { get; init;  }
    [Id(3)]public required string Name { get; init;  }
    [Id(4)]public required IssueStateResponse? VotingIssue { get; init;  }
    [Id(5)]public required ICollection<PlayerStateResponse> Players { get; init;  }
    [Id(6)]public required ICollection<IssueStateResponse> Issues { get; init;  }
    [Id(7)]public required IssueOrder IssueOrder { get; init;  }
}
