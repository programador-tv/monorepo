using Domain.Enumerables;
using Domain.Primitives;

namespace Domain.Entities;

public sealed class NotifyUserLiveEarly(
    Guid id,
    Guid liveId,
    Guid perfilId,
    bool active,
    bool hasNotificated
) : Entity(id)
{
    public Guid LiveId { get; private set; } = liveId;
    public Guid PerfilId { get; private set; } = perfilId;
    public bool Active { get; private set; } = active;
    public bool HasNotificated { get; private set; } = hasNotificated;

    public static NotifyUserLiveEarly Create(
        Guid LiveId,
        Guid PerfilId,
        bool Active,
        bool hasNotificated
    )
    {
        return new NotifyUserLiveEarly(Guid.NewGuid(), LiveId, PerfilId, Active, hasNotificated);
    }

    public void ActiveNotification()
    {
        Active = true;
    }

    public void DisableNotification()
    {
        Active = false;
    }

    public void MarkAsNotificated()
    {
        HasNotificated = true;
    }

    public Notification BuildUpcomingNotification(
        Guid perfilGerador,
        Guid liveId,
        string tituloLive
    )
    {
        return Notification.Create(
            PerfilId,
            perfilGerador,
            TipoNotificacao.EspectadorLiveProxima,
            $@"
                Hey, estou prestes a começar a live: {tituloLive}
            ",
            "/Watch?key=" + liveId,
            null
        );
    }
}
