namespace StoryPoker.Server.Silo.Configurations;

public class SiloConfig
{
    public required ClusterConfig ClusterConfig { get; init; }
    public required RedisPersistenceConfig RedisPersistenceConfig { get; init; }
}

public class ClusterConfig
{
    public required string ConnectionString { get; init; }
    public required string ClusterId { get; init; }
    public required string ServiceId { get; init; }
}

public class RedisPersistenceConfig
{
    public required string ConnectionString { get; init; }
}
