using Microsoft.AspNetCore.Mvc;
using InternalProject.Application;
using InternalProject.Contracts;
using InternalProject.Domain;

namespace InternalProject.Controllers;

[ApiController]
[Route("posts")]
public class PostsController : ControllerBase
{
    private readonly PostService _postService;

    public PostsController(PostService postService)
    {
        _postService = postService;
    }

    [HttpPost]
    public async Task<ActionResult<PostResponse>> Create(
        CreatePostRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _postService.CreatePostAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PostResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _postService.GetPostByIdAsync(id, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PostListItemResponse>>> List(
        [FromQuery] PostStatus? status,
        [FromQuery] Guid? authorId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _postService.ListPostsAsync(
            status,
            authorId,
            page,
            pageSize,
            cancellationToken);

        return Ok(result);
    }

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<PostResponse>> Update(
        Guid id,
        UpdatePostRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _postService.UpdatePostAsync(id, request, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPatch("{id:guid}/publish")]
    public async Task<ActionResult<PostResponse>> Publish(
        Guid id,
        PublishPostRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _postService.PublishPostAsync(id, request, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPatch("{id:guid}/unpublish")]
    public async Task<ActionResult<PostResponse>> Unpublish(
        Guid id,
        PublishPostRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _postService.UnpublishPostAsync(id, request, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }
}