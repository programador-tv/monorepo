using Domain.Contracts;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class CommentRepository(ApplicationDbContext context)
    : GenericRepository<Comment>(context),
        ICommentRepository
{
    public async Task<Comment> GetById(string commentId) =>
        await DbContext.Comments.Where(e => e.Id == Guid.Parse(commentId)).FirstAsync();

    public async Task Update(Comment comment)
    {
        DbContext.Comments.Update(comment);
        await DbContext.SaveChangesAsync();
    }
}
