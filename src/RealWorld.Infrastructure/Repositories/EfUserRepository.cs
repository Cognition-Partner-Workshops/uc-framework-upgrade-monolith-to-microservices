using Microsoft.EntityFrameworkCore;
using RealWorld.Core.Entities;
using RealWorld.Core.Interfaces;
using RealWorld.Infrastructure.Data;

namespace RealWorld.Infrastructure.Repositories;

public class EfUserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public EfUserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task SaveAsync(User user)
    {
        var existing = await _context.Users.FindAsync(user.Id);
        if (existing == null)
        {
            _context.Users.Add(user);
        }
        else
        {
            existing.Email = user.Email;
            existing.Username = user.Username;
            existing.Password = user.Password;
            existing.Bio = user.Bio;
            existing.Image = user.Image;
        }
        await _context.SaveChangesAsync();
    }

    public async Task<User?> FindByIdAsync(string id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> FindByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> FindByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task SaveRelationAsync(FollowRelation followRelation)
    {
        var existing = await FindRelationAsync(followRelation.UserId, followRelation.TargetId);
        if (existing == null)
        {
            _context.FollowRelations.Add(followRelation);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<FollowRelation?> FindRelationAsync(string userId, string targetId)
    {
        return await _context.FollowRelations
            .FirstOrDefaultAsync(f => f.UserId == userId && f.TargetId == targetId);
    }

    public async Task RemoveRelationAsync(FollowRelation followRelation)
    {
        _context.FollowRelations.Remove(followRelation);
        await _context.SaveChangesAsync();
    }
}
