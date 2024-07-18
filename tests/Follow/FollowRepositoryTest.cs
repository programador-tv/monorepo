using Azure.Core;
using Domain.Contracts;
using Domain.Entities;
using Domain.Enumerables;
using Domain.Repositories;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using NuGet.Frameworks;

namespace tests;

public class FollowRepositoryTests
{
    private readonly FollowRepository _repository;
    private readonly ApplicationDbContext _context;

    public FollowRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new FollowRepository(_context);
    }

    [Fact]
    public async Task Create_ShouldReturnFollow()
    {
        var guidFollowerId = Guid.NewGuid();
        var guidFollowingId = Guid.NewGuid();

        var newFollow = Follow.Create(guidFollowerId, guidFollowingId);

        await _repository.Create(newFollow);

        var result = _context.Follows.FirstOrDefault();

        Assert.NotNull(result);
        Assert.Equal(newFollow.FollowingId, result.FollowingId);
        Assert.Equal(newFollow.FollowerId, result.FollowerId);
        Assert.Equal(newFollow.Active, result.Active);
    }

    [Fact]
    public async Task Update_ShouldWorkFine()
    {
        var guidFollowerId = Guid.NewGuid();
        var guidFollowingId = Guid.NewGuid();

        var newFollow = Follow.Create(guidFollowerId, guidFollowingId);

        await _context.Follows.AddAsync(newFollow);
        await _context.SaveChangesAsync();

        var follow = _context.Follows.First(f =>
            f.FollowerId == guidFollowerId && f.FollowingId == guidFollowingId
        );

        var oldActive = follow.Active;
        follow.UnfollowUser();

        await _repository.Update(follow);

        var result = _context.Follows.First(f =>
            f.FollowerId == guidFollowerId && f.FollowingId == guidFollowingId
        );

        Assert.NotNull(result);
        Assert.Equal(newFollow.FollowingId, result.FollowingId);
        Assert.Equal(newFollow.FollowerId, result.FollowerId);
        Assert.NotEqual(oldActive, result.Active);
        Assert.False(result.Active);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnFollow()
    {
        var guidFollowerId = Guid.NewGuid();
        var guidFollowingId = Guid.NewGuid();

        var newFollow = Follow.Create(guidFollowerId, guidFollowingId);

        await _context.Follows.AddAsync(newFollow);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(guidFollowerId, guidFollowingId);

        Assert.NotNull(result);
        Assert.Equal(newFollow.FollowingId, result.FollowingId);
        Assert.Equal(newFollow.FollowerId, result.FollowerId);
    }

    [Fact]
    public async Task GetFollowersAsync()
    {
        var guidUserID = Guid.NewGuid();
        var guidFollowerId = Guid.NewGuid();

        var newFollow = Follow.Create(guidFollowerId, guidUserID);

        await _context.Follows.AddAsync(newFollow);
        await _context.SaveChangesAsync();

        var result = await _repository.GetFollowersAsync(guidUserID);

        Assert.IsType<int>(result);
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task GetFollowingAsync()
    {
        var guidUserID = Guid.NewGuid();
        var guidFollowing = Guid.NewGuid();

        var newFollow = Follow.Create(guidUserID, guidFollowing);

        await _context.Follows.AddAsync(newFollow);
        await _context.SaveChangesAsync();

        var result = await _repository.GetFollowingAsync(guidUserID);

        Assert.IsType<int>(result);
        Assert.Equal(1, result);
    }
}
