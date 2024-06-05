using ErrorOr;
using Newtonsoft.Json;
using StoryPoker.Server.Abstractions.Room.Models.Enums;

namespace StoryPoker.Server.Grains.RoomGrains.Models;

public record InternalIssue
{
    public Guid Id { get; init; }
    public required string Title { get; init; }
    [JsonProperty] public float? StoryPoints { get; private set; }
    [JsonProperty] public int? FibonacciStoryPoints  { get; private set; }
    public Dictionary<Guid, int> PlayerStoryPoints { get; } = [];
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
        var fibonacciSequence = GenerateFibonacciSequence(storyPoints.Value);
        foreach (var fib in fibonacciSequence.Where(fib => fib >= storyPoints.Value))
        {
            return fib;
        }

        return fibonacciSequence[^1];
    }
    private static IList<int> GenerateFibonacciSequence(float maxNumber)
    {
        var fibonacci = new List<int> { 0, 1 };
        while (true)
        {
            var next = fibonacci[^1] + fibonacci[^2];
            if (next > maxNumber)
            {
                fibonacci.Add(next);
                break;
            }
            fibonacci.Add(next);
        }
        return fibonacci;
    }
}
