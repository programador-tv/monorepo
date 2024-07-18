using Domain.Contracts;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetNotificationsByPerfilId(Guid destinationId);
        Task<Notification> SaveNotification(Notification notification);

        Task UpdateRangeAsync(List<Notification> notifications);
    }
}
