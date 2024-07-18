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

public class TagRepositoryTests
{
    private readonly TagRepository _repository;
    private readonly ApplicationDbContext _context;

    public TagRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new TagRepository(_context);
    }

    [Fact]
    public async Task GetTagRelationByKeyword_ShouldReturnResponse()
    {
        var keyword = "Carolina";

        var tag1 = Tag.AddForLive("Carolina", Guid.NewGuid().ToString());
        var tag2 = Tag.AddForLive("Teste", Guid.NewGuid().ToString());

        _context.Tags.AddRange(tag1, tag2);
        await _context.SaveChangesAsync();

        var result = await _repository.GetTagRelationByKeyword(keyword);

        Assert.NotNull(result);
        Assert.Single(result);

        foreach (var item in result)
        {
            Assert.Equal(item.Id, tag1.Id);
        }
    }

    [Fact]
    public async Task GetTagRelationByKeyword_ShouldReturnEmptyList()
    {
        var keyword = "Aleatório";

        var tag = Tag.AddForLive("Programador", Guid.NewGuid().ToString());

        _context.Tags.AddRange(tag);
        await _context.SaveChangesAsync();

        var result = await _repository.GetTagRelationByKeyword(keyword);

        Assert.Empty(result);
        Assert.Equal(0, result.Count);
    }
}
