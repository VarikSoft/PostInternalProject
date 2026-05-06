namespace InternalProject.Contracts;

public record PostResponse(
    Guid Id,
    string Title,
    string Body,
    Guid AuthorId,
    string Status,
    DateTime CreatedAt,
    DateTime? PublishedAt,
    int Version
    );
