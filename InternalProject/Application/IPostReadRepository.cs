using InternalProject.Domain;

namespace InternalProject.Application;

public interface IPostReadRepository
{
    Task<Post?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Post>> ListAsync(
        PostStatus? status,
        Guid? authorId,
        int page,
        int pageSize,
        CancellationToken cancellationToken);
}