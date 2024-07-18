using Domain.Contracts;

namespace Domain.WebServices;

public interface INotificationWebService
{
    Task SaveAndSend(CreateNotificationRequest request);
    Task<List<NotificationItemResponse>> GetById(Guid id);
}
