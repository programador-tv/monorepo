using Domain.Primitives;

namespace Domain.Entities;

public sealed class FreeTimeBackstage(
    Guid id,
    Guid timeSelectionId,
    int maxParticipants,
    bool ilimitado
) : Entity(id)
{
    public Guid TimeSelectionId { get; private set; } = timeSelectionId;
    public int MaxParticipants { get; private set; } = maxParticipants;
    public bool Ilimitado { get; private set; } = ilimitado;

    public static FreeTimeBackstage Create(
        Guid timeSelectionId,
        int maxParticipants,
        bool ilimitado
    )
    {
        return new FreeTimeBackstage(Guid.NewGuid(), timeSelectionId, maxParticipants, ilimitado);
    }
}
