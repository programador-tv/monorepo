using Domain.Contracts;
using Domain.Entities;
using Domain.Enumerables;
using Domain.Repositories;
using Domain.WebServices;
using Queue;

namespace Application.Logic;

public sealed class CommentBusinessLogic(
    ICommentRepository _repository,
    ILiveRepository _liveRepository,
    IPerfilRepository _perfilRepository,
    IMessagePublisher _publisher,
    IBocaSujaWebService bocaSujaWebService
) : ICommentBusinessLogic
{
    public async Task ValidateComment(string notificationId)
    {
        var comment = await _repository.GetById(notificationId);

        var live =
            await _liveRepository.GetByIdAsync(comment.LiveId)
            ?? throw new ArgumentException(notificationId);
        var liveOwner =
            await _perfilRepository.GetByIdAsync(live.PerfilId)
            ?? throw new ArgumentException(notificationId);

        var result = await bocaSujaWebService.Validate(
            comment.Content ?? string.Empty,
            liveOwner.Id
        );

        if (result.Contains("503 Service Temporarily Unavailable"))
        {
            throw new HttpRequestException($"503 Serviço temporariamente indisponível.");
        }
        if (result != "true")
        {
            throw new Exception();
        }

        comment.Validate();
        await _repository.Update(comment);

        var notification = Notification.Create(
            liveOwner.Id,
            comment.PerfilId,
            TipoNotificacao.ComentarioNoMeuVideo,
            $@" postou um comentário em seu video {live.Titulo}",
            "/Watch?key=" + comment.LiveId,
            null
        );

        await _publisher.PublishAsync("NotificationsQueue", notification);
    }
}
