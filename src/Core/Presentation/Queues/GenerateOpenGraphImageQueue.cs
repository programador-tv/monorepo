using Domain.Contracts;
using Domain.Entities;
using Domain.Enumerables;
using Domain.Utils;
using Domain.WebServices;
using Infrastructure.Browser;
using MassTransit;
using Queue;

namespace Background;

public sealed class GenerateOpenGraphImageQueue(
    ITimeSelectionWebService webService,
    IPublicWebService publicWebService,
    IBrowserHandler browser,
    IMessagePublisher publisher
) : IConsumer<UpdateTimeSelectionPreviewMessage>
{
    public async Task Consume(ConsumeContext<UpdateTimeSelectionPreviewMessage> context)
    {
        try
        {
            var id = context.Message.TimeSelectionId;
            var build = await webService.GetBuildOpenGraphImage(id);

            var perfilImageBase64 = await publicWebService.GetFotoPerfilBase64Async(
                build.UserNickName
            );
            var html = HtmlUtils.CreatePreviewForFreeTime(build, perfilImageBase64);
            var OpenGraphImage = await browser.BuildImageFromHtml(html, "#free-time-preview");

            var request = new UpdateTimeSelectionPreviewRequest(id, OpenGraphImage);
            await webService.UpdatePreview(request);

            var conteudo =
                $@" Sua mentoria foi criada e agendada com sucesso! 
                                    Já está tudo pronto para compartilhar:
                                    https://programador.tv/Canal?usr={build.UserNickName}&freetime={id}
                                ";

            var notification = Notification.Create(
                build.PerfilId,
                build.PerfilId,
                TipoNotificacao.PreviewCriada,
                conteudo,
                "https://programador.tv/Canal?usr={build.UserNickName}&freetime={id}",
                string.Empty
            );
            await publisher.PublishAsync(typeof(NotificationsQueue).Name, notification);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
