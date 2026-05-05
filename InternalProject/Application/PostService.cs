using InternalProject.Contracts;
using InternalProject.Domain;

namespace InternalProject.Application;

public class PostService
{
    private readonly IPostReadRepository _readRepository;
    private readonly IPostWriteRepository _writeRepository;

    public PostService(
        IPostReadRepository readRepository,
        IPostWriteRepository writeRepository)
    {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
    }

    public async Task<PostResponse> CreatePostAsync(
        CreatePostRequest request,
        CancellationToken cancellationToken)
    {
        var post = new Post(request.Title, request.Body, request.AuthorId);

        await _writeRepository.AddAsync(post, cancellationToken);
        await _writeRepository.SaveChangesAsync(cancellationToken);

        return MapToResponse(post);
    }

    public async Task<PostResponse?> GetPostByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var post = await _readRepository.GetByIdAsync(id, cancellationToken);

        return post is null ? null : MapToResponse(post);
    }

    public async Task<IReadOnlyList<PostListItemResponse>> ListPostsAsync(
        PostStatus? status,
        Guid? authorId,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var posts = await _readRepository.ListAsync(
            status,
            authorId,
            page,
            pageSize,
            cancellationToken);

        return posts.Select(MapToListItemResponse).ToList();
    }

    public async Task<PostResponse?> UpdatePostAsync(
        Guid id,
        UpdatePostRequest request,
        CancellationToken cancellationToken)
    {
        var post = await _readRepository.GetByIdAsync(id, cancellationToken);

        if (post is null) return null;

        post.Update(request.Title, request.Body, request.RequesterId);

        await _writeRepository.SaveChangesAsync(cancellationToken);

        return MapToResponse(post);
    }

    public async Task<PostResponse?> PublishPostAsync(
        Guid id,
        PublishPostRequest request,
        CancellationToken cancellationToken)
    {
        var post = await _readRepository.GetByIdAsync(id, cancellationToken);

        if (post is null) return null;

        post.Publish(request.RequesterId);

        await _writeRepository.SaveChangesAsync(cancellationToken);

        return MapToResponse(post);
    }

    public async Task<PostResponse?> UnpublishPostAsync(
        Guid id,
        PublishPostRequest request,
        CancellationToken cancellationToken)
    {
        var post = await _readRepository.GetByIdAsync(id, cancellationToken);

        if (post is null) return null;

        post.Unpublish(request.RequesterId);

        await _writeRepository.SaveChangesAsync(cancellationToken);

        return MapToResponse(post);
    }

    private static PostResponse MapToResponse(Post post)
    {
        return new PostResponse(
            post.Id,
            post.Title,
            post.Body,
            post.AuthorId,
            post.Status.ToString(),
            post.CreatedAt,
            post.PublishedAt);
    }

    private static PostListItemResponse MapToListItemResponse(Post post)
    {
        return new PostListItemResponse(
            post.Id,
            post.Title,
            post.AuthorId,
            post.Status.ToString(),
            post.CreatedAt,
            post.PublishedAt);
    }
}
