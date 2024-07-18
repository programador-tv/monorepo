using Domain.Models;

namespace Domain.Redis;

public interface IChatRedisRepository
{
    Task<List<CreateChatMessageRequest>> GetMessageAsync(Guid liveId);
    Task SaveMessageAsync(CreateChatMessageRequest message);
    Task DeleteMessageAsync(string messageId, Guid liveId);
}
