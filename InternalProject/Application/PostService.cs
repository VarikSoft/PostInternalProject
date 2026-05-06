using InternalProject.Contracts;
using InternalProject.Domain;

namespace InternalProject.Application;

public class PostService
{
    private readonly IPostReadRepository _readRepository;
    private readonly IPostWriteRepository _writeRepository;
    private readonly ISystemValueProvider _systemValueProvider;

    public PostService(
        IPostReadRepository readRepository,
        IPostWriteRepository writeRepository,
        ISystemValueProvider systemValueProvider)
    {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
        _systemValueProvider = systemValueProvider;
    }

    public async Task<PostResponse> CreatePostAsync(
        CreatePostRequest request,
        CancellationToken cancellationToken)
    {
        var post = new Post(
            request.Title,
            request.Body,
            request.AuthorId,
            _systemValueProvider.NewGuid(),
            _systemValueProvider.UtcNow);

        await _writeRepository.AddAsync(post, cancellationToken);
        await _writeRepository.SaveChangesAsync(cancellationToken);

        return post.ToResponse();
    }

    public async Task<PostResponse?> GetPostByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var post = await _readRepository.GetByIdAsync(id, cancellationToken);

        return post is null ? null : post.ToResponse();
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

        return posts.Select(post => post.ToListItemResponse()).ToList();
    }

    public async Task<PostResponse?> UpdatePostAsync(
        Guid id,
        UpdatePostRequest request,
        CancellationToken cancellationToken)
    {
        var post = await _readRepository.GetByIdAsync(id, cancellationToken);

        if (post is null) return null;

        post.Update(request.Title, request.Body, request.RequesterId, request.ExpectedVersion);

        await _writeRepository.SaveChangesAsync(cancellationToken);

        return post.ToResponse();
    }

    public async Task<PostResponse?> PublishPostAsync(
        Guid id,
        PublishPostRequest request,
        CancellationToken cancellationToken)
    {
        var post = await _readRepository.GetByIdAsync(id, cancellationToken);

        if (post is null) return null;

        post.Publish(request.RequesterId, request.ExpectedVersion, _systemValueProvider.UtcNow);

        await _writeRepository.SaveChangesAsync(cancellationToken);

        return post.ToResponse();
    }

    public async Task<PostResponse?> UnpublishPostAsync(
        Guid id,
        PublishPostRequest request,
        CancellationToken cancellationToken)
    {
        var post = await _readRepository.GetByIdAsync(id, cancellationToken);

        if (post is null) return null;

        post.Unpublish(request.RequesterId, request.ExpectedVersion);

        await _writeRepository.SaveChangesAsync(cancellationToken);

        return post.ToResponse();
    }
}
