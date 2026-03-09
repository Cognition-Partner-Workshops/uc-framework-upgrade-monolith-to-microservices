using RealWorld.Core.Entities;

namespace RealWorld.Core.Interfaces;

public interface IUserRepository
{
    Task SaveAsync(User user);
    Task<User?> FindByIdAsync(string id);
    Task<User?> FindByUsernameAsync(string username);
    Task<User?> FindByEmailAsync(string email);
    Task SaveRelationAsync(FollowRelation followRelation);
    Task<FollowRelation?> FindRelationAsync(string userId, string targetId);
    Task RemoveRelationAsync(FollowRelation followRelation);
}
