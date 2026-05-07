using InternalProject.Contracts;
using InternalProject.Domain;

namespace InternalProject.Application;

internal static class PostMappingExtensions
{
    public static PostResponse ToResponse(this Post post)
    {
        return new PostResponse(
            post.Id,
            post.Title,
            post.Body,
            post.AuthorId,
            post.Status.ToString(),
            post.CreatedAt,
            post.PublishedAt,
            post.Version);
    }

    public static PostListItemResponse ToListItemResponse(this Post post)
    {
        return new PostListItemResponse(
            post.Id,
            post.Title,
            post.AuthorId,
            post.Status.ToString(),
            post.CreatedAt,
            post.PublishedAt,
            post.Version);
    }
}
