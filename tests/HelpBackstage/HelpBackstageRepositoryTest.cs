using Azure.Core;
using Domain.Contracts;
using Domain.Entities;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace tests;

public class HelpBackstageRepositoryTest
{
    private readonly HelpBackstageRepository _repository;
    private readonly ApplicationDbContext _context;

    public HelpBackstageRepositoryTest()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new HelpBackstageRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddHelpBackstage()
    {
        var fakeTSId = Guid.NewGuid();
        var fakeDescription = "FAKE_DESCRICAO";
        var requestBackstage = new CreateHelpBackstageRequest(fakeTSId, fakeDescription);

        var backstage = HelpBackstage.Create(requestBackstage);

        await _repository.AddAsync(backstage);

        var result = await _context.HelpBackstages.FindAsync(backstage.Id);

        Assert.Multiple(() =>
        {
            Assert.NotNull(result);
            Assert.Equal(backstage.TimeSelectionId, result.TimeSelectionId);
            Assert.Equal(backstage.Descricao, result.Descricao);
            Assert.Null(result.ImagePath);
        });
    }

    [Fact]
    public async Task GetByTimeSelectionIdAsync_ShouldReturnRelationedHelpBackstage()
    {
        var fakeTSId = Guid.NewGuid();
        var fakeDescription = "FAKE_DESCRICAO";
        var requestBackstage = new CreateHelpBackstageRequest(fakeTSId, fakeDescription);

        var backstage = HelpBackstage.Create(requestBackstage);

        await _repository.AddAsync(backstage);

        var result = await _repository.GetByTimeSelectionIdAsync(fakeTSId);

        Assert.Multiple(() =>
        {
            Assert.NotNull(result);
            Assert.Equal(backstage.TimeSelectionId, result.TimeSelectionId);
            Assert.Equal(backstage.Descricao, result.Descricao);
            Assert.Null(result.ImagePath);
        });
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateHelpBackstage()
    {
        var fakeTSId = Guid.NewGuid();
        var fakeDescription = "FAKE_DESCRICAO";
        var fakeImgPath = "shared\\RequestHelp\\imageFile";
        var requestBackstage = new CreateHelpBackstageRequest(fakeTSId, fakeDescription);

        var backstage = HelpBackstage.Create(requestBackstage);

        await _repository.AddAsync(backstage);

        backstage.AddImagePath(fakeImgPath);
        await _repository.UpdateAsync(backstage);
        var result = await _context.HelpBackstages.FindAsync(backstage.Id);

        if (result != null)
        {
            Assert.Equal(fakeImgPath, result.ImagePath);
        }
    }

    [Fact]
    public async Task GetByIdsAsync_ShouldReturnAHelpBackstageList()
    {
        _context.HelpBackstages.RemoveRange(_context.HelpBackstages.ToList());
        _context.HelpBackstages.AddRange(
            HelpBackstage.Create(new(Guid.NewGuid(), "fake_description")),
            HelpBackstage.Create(new(Guid.NewGuid(), "fake_description")),
            HelpBackstage.Create(new(Guid.NewGuid(), "fake_description"))
        );
        _context.SaveChanges();
        var tsIds = _context.HelpBackstages.Select(hb => hb.TimeSelectionId).ToList();
        var result = await _repository.GetByIdsAsync(tsIds);

        Assert.Multiple(() =>
        {
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(3, result.Count);
        });
    }
}
