using Domain.Contracts;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class NotificationRepository(ApplicationDbContext context)
    : GenericRepository<Notification>(context),
        INotificationRepository
{
    public async Task<List<Notification>> GetNotificationsByPerfilId(Guid destinationId)
    {
        return await DbContext
            .Notifications.Where(e => e.DestinoPerfilId == destinationId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<Notification> SaveNotification(Notification notification)
    {
        await DbContext.Notifications.AddAsync(notification);
        await DbContext.SaveChangesAsync();
        return notification;
    }

    public async Task UpdateRangeAsync(List<Notification> notifications)
    {
        DbContext.Notifications.UpdateRange(notifications);
        await DbContext.SaveChangesAsync();
    }
}
