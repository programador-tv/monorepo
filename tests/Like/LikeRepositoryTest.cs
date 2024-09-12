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

public class LikeRepositoryTests
{
    private readonly LikeRepository _repository;
    private readonly ApplicationDbContext _context;

    public LikeRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new LikeRepository(_context);
    }

    [Fact]
    public async Task GetAllLikesByLiveId_ShouldReturnListOfLikes()
    {
        var liveId = Guid.NewGuid();

        var relatedUserIdOne = Guid.NewGuid();
        var relatedUserIdTwo = Guid.NewGuid();

        var newLikeOne = Like.Create(liveId, relatedUserIdOne);
        var newLikeTwo = Like.Create(liveId, relatedUserIdTwo);

        await _context.Likes.AddAsync(newLikeOne);
        await _context.Likes.AddAsync(newLikeTwo);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllLikesByLiveId(liveId);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        foreach (var like in result)
        {
            Assert.Equal(liveId, like.EntityId);
            Assert.True(
                like.RelatedUserId == relatedUserIdOne || like.RelatedUserId == relatedUserIdTwo
            );
        }
    }

    [Fact]
    public async Task CreateLike_ShouldAddLike()
    {
        var entityId = Guid.NewGuid();
        var relatedUserId = Guid.NewGuid();
        var newLike = Like.Create(entityId, relatedUserId);

        await _repository.CreateLike(newLike);

        var result = await _context.Likes.FindAsync(newLike.Id);

        Assert.NotNull(result);
        Assert.Equal(newLike.Id, result.Id);
        Assert.Equal(newLike.EntityId, result.EntityId);
        Assert.Equal(newLike.RelatedUserId, result.RelatedUserId);
    }
}
