namespace RealWorld.Application.Services;

public class TagsQueryService
{
    private readonly ITagReadService _tagReadService;

    public TagsQueryService(ITagReadService tagReadService)
    {
        _tagReadService = tagReadService;
    }

    public async Task<List<string>> AllTagsAsync()
    {
        return await _tagReadService.AllAsync();
    }
}
