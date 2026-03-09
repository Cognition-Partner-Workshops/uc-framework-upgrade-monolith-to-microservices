namespace RealWorld.Application.Services;

public interface ITagReadService
{
    Task<List<string>> AllAsync();
}
