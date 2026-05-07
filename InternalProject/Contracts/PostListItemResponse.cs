namespace InternalProject.Contracts;

public record PostListItemResponse(
    Guid Id,
    string Title,
    Guid AuthorId,
    string Status,
    DateTime CreatedAt,
    DateTime? PublishedAt,
    int Version
    );
