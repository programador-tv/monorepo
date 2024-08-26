using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enumerables;
using Domain.Enums;
using Domain.Primitives;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Index(nameof(StartTime))]
public sealed class TimeSelection(
    Guid id,
    Guid perfilId,
    Guid? roomId,
    DateTime startTime,
    DateTime endTime,
    string? tituloTemporario,
    EnumTipoTimeSelection tipo,
    StatusTimeSelection status,
    bool notifiedMentoriaProxima,
    string? previewImage,
    TipoAction tipoAction,
    Variacao variacao,
    DateTime createdAt
) : Entity(id, createdAt)
{
    public Guid PerfilId { get; private set; } = perfilId;
    public Guid? RoomId { get; private set; } = roomId;
    public DateTime StartTime { get; private set; } = startTime;
    public DateTime EndTime { get; private set; } = endTime;
    public string? TituloTemporario { get; private set; } = tituloTemporario;
    public EnumTipoTimeSelection Tipo { get; private set; } = tipo;
    public StatusTimeSelection Status { get; private set; } = status;
    public bool NotifiedMentoriaProxima { get; private set; } = notifiedMentoriaProxima;
    public string? PreviewImage { get; private set; } = previewImage;
    public TipoAction TipoAction { get; private set; } = tipoAction;
    public Variacao Variacao { get; private set; } = variacao;

    public static TimeSelection Create(
        Guid perfilId,
        Guid? roomId,
        DateTime startTime,
        DateTime endTime,
        string? tituloTemporario,
        EnumTipoTimeSelection tipo,
        TipoAction tipoAction,
        Variacao variacao
    )
    {
        return new TimeSelection(
            Guid.NewGuid(),
            perfilId,
            roomId,
            startTime,
            endTime,
            tituloTemporario,
            tipo,
            StatusTimeSelection.Pendente,
            false,
            string.Empty,
            tipoAction,
            variacao,
            DateTime.Now
        );
    }

    public Notification BuildUpcomingNotification(Guid JoinTimePerfilId)
    {
        return Notification.Create(
            PerfilId,
            JoinTimePerfilId,
            TipoNotificacao.MentorMentoriaProxima,
            $@"
                Nossa mentoria está próxima :)
            ",
            "/?event=" + Id,
            null
        );
    }

    public Notification BuildPendingNotification(Guid JoinTimePerfilId)
    {
        return Notification.Create(
            PerfilId,
            JoinTimePerfilId,
            TipoNotificacao.MentorConsideracaoFinalPendente,
            $@"
                Considerações finais pendentes para {TituloTemporario}
            ",
            "/?event=" + Id,
            null
        );
    }

    public void ChangeStatus(StatusTimeSelection status)
    {
        Status = status;
    }

    public void MarkAsNotified()
    {
        NotifiedMentoriaProxima = true;
    }

    public void ChangePreviewImage(string? previewImage)
    {
        PreviewImage = previewImage;
    }

    public void ChangeTituloTemporario(string? tituloTemporario)
    {
        TituloTemporario = tituloTemporario;
    }
}
