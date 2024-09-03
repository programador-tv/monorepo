using System;
using Domain.Contracts;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class FollowRepository(ApplicationDbContext context)
    : GenericRepository<Follow>(context),
        IFollowRepository
{
    public async Task<Follow?> GetByIdAsync(Guid followerId, Guid followingId) =>
        await DbContext.Follows.FirstOrDefaultAsync(f =>
            f.FollowerId == followerId && f.FollowingId == followingId
        );

    public async Task Create(Follow follow)
    {
        await DbContext.Follows.AddAsync(follow);
        await DbContext.SaveChangesAsync();
    }

    public async Task<bool> Update(Follow follow)
    {
        DbContext.Follows.Update(follow);
        await DbContext.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetFollowersAsync(Guid userId)
    {
        return await DbContext.Follows.Where(f => f.FollowingId == userId && f.Active).CountAsync();
    }

    public async Task<List<FollowersResponse>> GetFollowersByIdsAsync(List<Guid> usersId)
    {
        return await DbContext
            .Follows.Where(f => usersId.Contains(f.FollowingId) && f.Active)
            .GroupBy(f => f.FollowingId)
            .Select(g => new FollowersResponse(g.Key, g.Count()))
            .ToListAsync();
    }

    public async Task<int> GetFollowingAsync(Guid userId)
    {
        return await DbContext.Follows.Where(f => f.FollowerId == userId && f.Active).CountAsync();
    }

    public async Task<bool> IsFollowingAsync(Guid followerId, Guid followingId)
    {
        return await DbContext.Follows.AnyAsync(
            f => f.FollowerId == followerId && f.FollowingId == followingId && f.Active);
    }
}
