namespace StoryPoker.Client.Web.Api.Configurations;
public class ClusterConfig
{
    public required string ConnectionString { get; init; }
    public required string ClusterId { get; init; }
    public required string ServiceId { get; init; }
}
