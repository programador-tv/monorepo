using Domain.Contracts;
using Domain.Entities;

namespace Application.Logic;

public interface IChatBusinessLogic
{
    Task SaveAsync(ChatMessage chatMessage);
}
