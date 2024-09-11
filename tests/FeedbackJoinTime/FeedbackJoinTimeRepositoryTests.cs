using Domain.Contracts;
using Domain.Entities;
using Domain.Enumerables;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;

namespace tests;

public class FeedbackTimeJoinRepositoryTests
{
    private readonly FeedbackJoinTimeRepository _repository;
    private readonly ApplicationDbContext _context;

    public FeedbackTimeJoinRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new FeedbackJoinTimeRepository(_context);
    }

    [Fact]
    public async Task AddFeedbackJoinTime_ShouldReturnAFeedbackJoinTime()
    {
        var entity = FeedbackJoinTime.Create(Guid.NewGuid(), DateTime.Now);
        var add = await _repository.AddFeedbackJoinTime(entity);
        Assert.NotNull(add);
        Assert.IsType<FeedbackJoinTime>(add);
    }
}
