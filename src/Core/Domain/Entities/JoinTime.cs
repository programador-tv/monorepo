using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enumerables;
using Domain.Enums;
using Domain.Primitives;

namespace Domain.Entities;

public sealed class JoinTime(
    Guid id,
    Guid perfilId,
    Guid timeSelectionId,
    StatusJoinTime statusJoinTime,
    bool notifiedMentoriaProxima,
    TipoAction tipoAction
) : Entity(id)
{
    public Guid PerfilId { get; private set; } = perfilId;
    public Guid TimeSelectionId { get; private set; } = timeSelectionId;
    public StatusJoinTime StatusJoinTime { get; private set; } = statusJoinTime;
    public bool NotifiedMentoriaProxima { get; private set; } = notifiedMentoriaProxima;

    public TipoAction TipoAction { get; set; } = tipoAction;

    public static JoinTime Create(
        Guid perfilId,
        Guid timeSelectionId,
        StatusJoinTime statusJoinTime,
        bool notifiedMentoriaProxima,
        TipoAction tipoAction
    )
    {
        return new JoinTime(
            Guid.NewGuid(),
            perfilId,
            timeSelectionId,
            statusJoinTime,
            notifiedMentoriaProxima,
            tipoAction
        );
    }

    public void ChangeStatus(StatusJoinTime statusJoinTime)
    {
        StatusJoinTime = statusJoinTime;
    }

    public void MarkAsNotified()
    {
        NotifiedMentoriaProxima = true;
    }

    public Notification BuildUpcomingNotification(Guid TimeSelectionPerfilId)
    {
        return Notification.Create(
            PerfilId,
            TimeSelectionPerfilId,
            TipoNotificacao.AlunoMentoriaProxima,
            $@"
                Nossa mentoria está próxima :)
            ",
            "/?event=" + Id,
            null
        );
    }

    public Notification BuildPendingNotification(Guid TimeSelectionPerfilId, string title)
    {
        return Notification.Create(
            PerfilId,
            TimeSelectionPerfilId,
            TipoNotificacao.AlunoConsideracaoFinalPendente,
            $@"
                Considerações finais pendentes para {title}
            ",
            "/?event=" + Id,
            null
        );
    }
}
