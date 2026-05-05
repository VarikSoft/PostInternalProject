namespace InternalProject.Contracts;

public record UpdatePostRequest(
    string Title,
    string Body,
    Guid RequesterId
    );