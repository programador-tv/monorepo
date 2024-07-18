using Domain.Contracts;
using Domain.Entities;
using Domain.WebServices;
using MassTransit;

namespace Background;

public sealed class NotificationsQueue(INotificationWebService webService) : IConsumer<Notification>
{
    public async Task Consume(ConsumeContext<Notification> context)
    {
#warning deveria estar sendo recebido ja o request ao invés da entidade

        var request = new CreateNotificationRequest(
            context.Message.DestinoPerfilId,
            context.Message.GeradorPerfilId,
            context.Message.TipoNotificacao,
            context.Message.Conteudo,
            context.Message.ActionLink,
            context.Message.SecundaryLink
        );

        var SaveAndSendNotification = webService.SaveAndSend(request);

        try
        {
            await SaveAndSendNotification;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
