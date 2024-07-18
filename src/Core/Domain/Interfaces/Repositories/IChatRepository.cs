using Domain.Contracts;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface IChatRepository
    {
        Task SaveAsync(ChatMessage chatMessage);
    }
}
