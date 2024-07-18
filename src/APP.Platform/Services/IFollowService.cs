using Domain.Entities;
using Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Mvc;

namespace Platform.Services;

public interface IFollowService
{
    Task<bool> CreateOrToggleFollower(Guid followerId, Guid followingId);
    Task<bool> IsFollowingAsync(Guid followerId, Guid followingId);
    Task<List<Follow>> GetFollowersAsync(Guid userId);
    Task<List<Follow>> GetFollowingAsync(Guid userId);
}
