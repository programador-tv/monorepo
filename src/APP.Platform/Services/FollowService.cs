using Domain.Entities;
using Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Platform.Services;

public class FollowService : IFollowService
{
    private readonly ApplicationDbContext _applicationDbContext;

    public FollowService(ApplicationDbContext dbContext)
    {
        _applicationDbContext = dbContext;
    }

    public async Task<bool> CreateOrToggleFollower(Guid followerId, Guid followingId)
    {
        var existingFollow = await _applicationDbContext.Follows.FirstOrDefaultAsync(f =>
            f.FollowerId == followerId && f.FollowingId == followingId
        );

        if (existingFollow == null)
        {
            var newFollow = new Follow
            {
                FollowerId = followerId,
                FollowingId = followingId,
                Active = true,
            };

            _applicationDbContext.Follows.Add(newFollow);
        }
        else
        {
            existingFollow.Active = !existingFollow.Active;
        }

        await _applicationDbContext.SaveChangesAsync();

        return existingFollow == null || existingFollow.Active;
    }

    public async Task<List<Follow>> GetFollowersAsync(Guid userId)
    {
        return await _applicationDbContext
            .Follows.Where(f => f.FollowingId == userId && f.Active)
            .ToListAsync();
    }

    public Task<List<Follow>> GetFollowingAsync(Guid userId)
    {
        return _applicationDbContext
            .Follows.Where(f => f.FollowerId == userId && f.Active)
            .ToListAsync();
    }

    public async Task<bool> IsFollowingAsync(Guid followerId, Guid followingId)
    {
        return await _applicationDbContext.Follows.AnyAsync(f =>
            f.FollowerId == followerId && f.FollowingId == followingId && f.Active
        );
    }
}
