using Microsoft.AspNetCore.Mvc;
using RealWorld.Application.Services;

namespace RealWorld.Api.Controllers;

[ApiController]
public class TagsController : ControllerBase
{
    private readonly TagsQueryService _tagsQueryService;

    public TagsController(TagsQueryService tagsQueryService)
    {
        _tagsQueryService = tagsQueryService;
    }

    [HttpGet("/tags")]
    public async Task<IActionResult> GetTags()
    {
        var tags = await _tagsQueryService.AllTagsAsync();
        return Ok(new { tags });
    }
}
