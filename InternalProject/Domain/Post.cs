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

    private Post()
    {
    }

    public Post(string title, string body, Guid authorId)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required");
        if (string.IsNullOrWhiteSpace(body)) throw new ArgumentException("Body is required");

        Id = Guid.NewGuid();
        Title = title;
        Body = body;
        AuthorId = authorId;
        Status = PostStatus.Draft;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string title, string body, Guid requesterId)
    {
        EnsureOwner(requesterId);

        if (Status == PostStatus.Published) throw new InvalidOperationException("Published post cannot be edited directly");
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required");
        if (string.IsNullOrWhiteSpace(body)) throw new ArgumentException("Body is required");

        Title = title;
        Body = body;
    }

    public void Publish(Guid requesterId)
    {
        EnsureOwner(requesterId);

        if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Body)) throw new InvalidOperationException("Cannot publish empty post.");

        Status = PostStatus.Published;
        PublishedAt = DateTime.UtcNow;
    }

    public void Unpublish(Guid requesterId)
    {
        EnsureOwner(requesterId);

        Status = PostStatus.Unpublished;
    }

    private void EnsureOwner(Guid requesterId)
    {
        if (AuthorId != requesterId) throw new UnauthorizedAccessException("Only author can modify this post");
    }
}
