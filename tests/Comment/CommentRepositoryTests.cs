using Domain.Entities;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace tests;

public class CommentRepositoryTests
{
    private readonly CommentRepository _repository;
    private readonly ApplicationDbContext _context;

    public CommentRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new CommentRepository(_context);
    }

    [Fact]
    public async Task GetAllByLiveIdAndPerfilId_ShouldReturnListOfComments()
    {
        var liveId = Guid.NewGuid();
        var perfilId = Guid.NewGuid();

        var newCommentOne = Comment.Create(perfilId, liveId, "comentário teste 1");
        var newCommentTwo = Comment.Create(perfilId, liveId, "comentário teste 2");

        await _context.Comments.AddAsync(newCommentOne);
        await _context.Comments.AddAsync(newCommentTwo);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllByLiveIdAndPerfilId(liveId, perfilId);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        foreach (var comment in result)
        {
            Assert.Equal(liveId, comment.LiveId);
            Assert.Equal(perfilId, comment.PerfilId);
        }
    }
}
