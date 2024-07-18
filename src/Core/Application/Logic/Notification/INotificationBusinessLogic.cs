using Domain.Contracts;
using Domain.Entities;

namespace Application.Logic;

public interface INotificationBusinessLogic
{
    Task NotifyWithServices(Notification notification);
    Task<Notification> SaveNotification(CreateNotificationRequest request);
    Task<List<NotificationItemResponse>> GetNotificationsByPerfilId(Guid destinationId);

    Task<List<NotificationItemResponse>> GetNotificationsAndMarkAsViewed(Guid destinationId);
}
