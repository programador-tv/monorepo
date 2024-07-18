using Domain.Contracts;
using Domain.Entities;
using Domain.Repositories;
using Queue;

namespace Application.Logic;

public sealed class NotificationBusinessLogic(
    INotificationRepository _repository,
    IPerfilRepository _perfilRepository,
    IMessagePublisher _publisher
) : INotificationBusinessLogic
{
    public async Task<List<NotificationItemResponse>> GetNotificationsByPerfilId(Guid destinationId)
    {
        var notifications = await _repository.GetNotificationsByPerfilId(destinationId);

        return notifications
            .Select(e => new NotificationItemResponse(
                e.DestinoPerfilId,
                e.GeradorPerfilId,
                e.TipoNotificacao,
                e.Vizualizado,
                e.DataCriacao,
                e.Conteudo,
                e.ActionLink,
                e.SecundaryLink
            ))
            .ToList();
    }

    public async Task NotifyWithServices(Notification notification)
    {
        var perfils = await _perfilRepository.GetByIdsAsync(
            [notification.DestinoPerfilId, notification.GeradorPerfilId]
        );

        var destino = perfils.First(e => e.Id == notification.DestinoPerfilId);
        var gerador = perfils.First(e => e.Id == notification.GeradorPerfilId);

        var mail = notification.BuildEmail(destino, gerador);

        if (mail == null)
        {
            return;
        }

        await _publisher.PublishAsync("EmailsQueue", mail);
    }

    public async Task<Notification> SaveNotification(CreateNotificationRequest request)
    {
        var notification = Notification.Create(
            request.DestinoPerfilId,
            request.GeradorPerfilId,
            request.TipoNotificacao,
            request.Conteudo,
            request.ActionLink,
            request.SecundaryLink
        );

        return await _repository.SaveNotification(notification);
    }

    public async Task<List<NotificationItemResponse>> GetNotificationsAndMarkAsViewed(
        Guid destinationId
    )
    {
        var notifications = await _repository.GetNotificationsByPerfilId(destinationId);
        var result = notifications
            .Select(e => new NotificationItemResponse(
                e.DestinoPerfilId,
                e.GeradorPerfilId,
                e.TipoNotificacao,
                e.Vizualizado,
                e.DataCriacao,
                e.Conteudo,
                e.ActionLink,
                e.SecundaryLink
            ))
            .ToList();

        if (notifications.Any(e => !e.Vizualizado))
        {
            var notViewed = notifications.Where(e => !e.Vizualizado).ToList();
            foreach (var notification in notViewed)
            {
                notification.Visualizar();
            }
            await _repository.UpdateRangeAsync(notViewed);
        }

        return result;
    }
}
