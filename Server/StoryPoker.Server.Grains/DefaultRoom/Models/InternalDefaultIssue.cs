using ErrorOr;
using Newtonsoft.Json;
using StoryPoker.Server.Abstractions.DefaultRoom.Models.Enums;
using StoryPoker.Server.Grains.Constants;

namespace StoryPoker.Server.Grains.DefaultRoom.Models;

public record InternalDefaultIssue
{
    public Guid Id { get; init; }
    public required string Title { get; init; }
    [JsonProperty] public float? StoryPoints { get; private set; }
    [JsonProperty] public int? FibonacciStoryPoints  { get; private set; }
    public IDictionary<Guid, int> PlayerStoryPoints { get; } = new Dictionary<Guid, int>();
    public required int Order { get; set; }
    public required VotingStage Stage { get; set; }

    public ErrorOr<Success> StartVote()
    {
        if (Stage == VotingStage.Voting)
            return Error.Failure(description: "Голосование уже началось");
        PlayerStoryPoints.Clear();
        Stage = VotingStage.Voting;
        return Result.Success;
    }

    public ErrorOr<Success> StartEndingVote()
    {
        if (Stage != VotingStage.Voting)
            return Error.Failure(description: "Голосование еще не началось");
        Stage = VotingStage.VoteEnding;
        return Result.Success;
    }

    public ErrorOr<Success> StopVote()
    {
        if (Stage != VotingStage.VoteEnding)
            return Error.Failure(description: "Таймер остановки не был запущен");

        Stage = VotingStage.VoteEnded;
        RecalculateStoryPoints();
        return Result.Success;
    }

    public void RecalculateStoryPoints()
    {
        StoryPoints = CalculateStoryPoints();
        FibonacciStoryPoints = CalculateFibonacciStoryPoints(StoryPoints);
    }

    internal bool CanChangeVotingIssue() => Stage is VotingStage.NotStarted or VotingStage.VoteEnded;
    internal bool CanRemove() => Stage is VotingStage.Voting or VotingStage.VoteEnding;
    private float? CalculateStoryPoints() => PlayerStoryPoints.Count == 0
        ? null
        : (float)Math.Round(PlayerStoryPoints.Average(x => x.Value),1);

    private static int? CalculateFibonacciStoryPoints(float? storyPoints)
    {
        if (!storyPoints.HasValue)
            return null;
        foreach (var fib in Fibonacci.Sequence.Where(fib => fib >= storyPoints.Value))
        {
            return fib;
        }

        return Fibonacci.Sequence[^1];
    }
}
