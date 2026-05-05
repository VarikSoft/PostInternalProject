using InternalProject.Domain;

namespace InternalProject.Application;

public interface IPostWriteRepository
{
    Task AddAsync(Post post, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
