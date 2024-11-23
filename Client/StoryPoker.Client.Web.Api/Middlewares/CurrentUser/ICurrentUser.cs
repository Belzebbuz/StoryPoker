namespace StoryPoker.Client.Web.Api.Middlewares.CurrentUser;

public interface ICurrentUser
{
    public Guid UserId { get; }
    public string? Name { get; }
}
