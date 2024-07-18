using Domain.Contracts;
using Domain.Entities;

namespace Application.Logic;

public interface ICommentBusinessLogic
{
    Task ValidateComment(string notificationId);
}
