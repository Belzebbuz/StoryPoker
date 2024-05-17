using ErrorOr;
using Newtonsoft.Json;
using StoryPoker.Server.Abstractions.Room.Models;
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
            return Error.Failure(description: "Голосвание уже началось");
        PlayerStoryPoints.Clear();
        Stage = VotingStage.Voting;
        return Result.Success;
    }

    public ErrorOr<Success> StopVote()
    {
        if (Stage != VotingStage.Voting)
            return Error.Failure(description: "Голосование не начато");

        Stage = VotingStage.VoteEnded;
        RecalculateStoryPoints();
        return Result.Success;
    }

    public void RecalculateStoryPoints()
    {
        StoryPoints = CalculateStoryPoints();
        FibonacciStoryPoints = CalculateFibonacciStoryPoints();
    }

    private float? CalculateStoryPoints() => PlayerStoryPoints.Count == 0
        ? null
        : (float)Math.Round(PlayerStoryPoints.Average(x => x.Value),1);

    private int? CalculateFibonacciStoryPoints()
    {
        if (!StoryPoints.HasValue)
            return null;
        var fibonacciSequence = GenerateFibonacciSequence(StoryPoints.Value);
        foreach (var fib in fibonacciSequence.Where(fib => fib >= StoryPoints.Value))
        {
            return fib;
        }

        return fibonacciSequence[^1];
    }
    static IList<int> GenerateFibonacciSequence(double maxNumber)
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
