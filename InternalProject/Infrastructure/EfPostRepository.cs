using Microsoft.EntityFrameworkCore;
using InternalProject.Application;
using InternalProject.Data;
using InternalProject.Domain;

namespace InternalProject.Infrastructure;

public class EfPostRepository : IPostReadRepository, IPostWriteRepository
{
    private readonly AppDbContext _dbContext;

    public EfPostRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Post?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Posts
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Post>> ListAsync(
        PostStatus? status,
        Guid? authorId,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        page = page <= 0 ? 1 : page;
        pageSize = pageSize <= 0 ? 20 : pageSize;
        pageSize = pageSize > 100 ? 100 : pageSize;

        IQueryable<Post> query = _dbContext.Posts;

        if (status is not null)
            query = query.Where(p => p.Status == status);

        if (authorId is not null)
            query = query.Where(p => p.AuthorId == authorId);

        return await query
            .OrderByDescending(p => p.PublishedAt ?? p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Post post, CancellationToken cancellationToken)
    {
        await _dbContext.Posts.AddAsync(post, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyConflictException("Post was modified by another user. Reload it and try again.", ex);
        }
    }
}
