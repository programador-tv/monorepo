using Domain.Contracts;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class ChatRepository(ApplicationDbContext context)
    : GenericRepository<Comment>(context),
        IChatRepository
{
    public async Task SaveAsync(ChatMessage chatMessage)
    {
        await DbContext.ChatMessages.AddAsync(chatMessage);
        await DbContext.SaveChangesAsync();
    }
}
