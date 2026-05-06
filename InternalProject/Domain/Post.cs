namespace InternalProject.Domain;

public class Post
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Body { get; private set; } = string.Empty;
    public Guid AuthorId { get; private set; }
    public PostStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? PublishedAt { get; private set; }
    public int Version { get; private set; }

    private Post()
    {
    }

    public Post(string title, string body, Guid authorId, Guid id, DateTime createdAt)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required");
        if (string.IsNullOrWhiteSpace(body)) throw new ArgumentException("Body is required");

        Id = id;
        Title = title;
        Body = body;
        AuthorId = authorId;
        Status = PostStatus.Draft;
        CreatedAt = createdAt;
        Version = 1;
    }

    public void Update(string title, string body, Guid requesterId, int expectedVersion)
    {
        EnsureOwner(requesterId);
        EnsureVersion(expectedVersion);

        if (Status == PostStatus.Published) throw new InvalidOperationException("Published post cannot be edited directly");
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required");
        if (string.IsNullOrWhiteSpace(body)) throw new ArgumentException("Body is required");

        Title = title;
        Body = body;
        Version++;
    }

    public void Publish(Guid requesterId, int expectedVersion, DateTime publishedAt)
    {
        EnsureOwner(requesterId);
        EnsureVersion(expectedVersion);

        if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Body)) throw new InvalidOperationException("Cannot publish empty post.");

        Status = PostStatus.Published;
        PublishedAt = publishedAt;
        Version++;
    }

    public void Unpublish(Guid requesterId, int expectedVersion)
    {
        EnsureOwner(requesterId);
        EnsureVersion(expectedVersion);

        Status = PostStatus.Unpublished;
        Version++;
    }

    private void EnsureOwner(Guid requesterId)
    {
        if (AuthorId != requesterId) throw new UnauthorizedAccessException("Only author can modify this post");
    }

    private void EnsureVersion(int expectedVersion)
    {
        if (expectedVersion <= 0) throw new ArgumentException("Expected version is required");

        if (Version != expectedVersion)
            throw new ConcurrencyConflictException("Post was modified by another user. Reload it and try again.");
    }
}
