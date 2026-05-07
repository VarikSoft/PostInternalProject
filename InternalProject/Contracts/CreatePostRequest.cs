namespace InternalProject.Contracts;

public record CreatePostRequest(
    string Title,
    string Body,
    Guid AuthorId
    );