namespace RealWorld.Application.Services;

public interface IUserRelationshipQueryService
{
    Task<bool> IsUserFollowingAsync(string userId, string anotherUserId);
    Task<HashSet<string>> FollowingAuthorsAsync(string userId, List<string> ids);
    Task<List<string>> FollowedUsersAsync(string userId);
}
