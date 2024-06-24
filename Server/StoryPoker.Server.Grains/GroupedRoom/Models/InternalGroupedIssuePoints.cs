using Newtonsoft.Json;

namespace StoryPoker.Server.Grains.GroupedRoom.Models;

public record InternalGroupedIssuePoints
{
    [JsonProperty] public float? StoryPoints { get; private set; }
    [JsonProperty] public int? FibonacciStoryPoints { get; private set; }

    public void Update(float? storyPoints, int? fibonacci)
    {
        StoryPoints = storyPoints;
        FibonacciStoryPoints = fibonacci;
    }
}
