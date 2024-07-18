using Domain.Primitives;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Index(nameof(LiveId), nameof(TimeSelectionId))]
public sealed class LiveBackstage(
    Guid id,
    Guid TimeSelectionId,
    Guid LiveId,
    string? TituloTemporario,
    string? Descricao
) : Entity(id)
{
    public Guid TimeSelectionId { get; private set; } = TimeSelectionId;
    public Guid LiveId { get; private set; } = LiveId;
    public string? TituloTemporario { get; private set; } = TituloTemporario;
    public string? Descricao { get; private set; } = Descricao;

    public static LiveBackstage Create(
        Guid TimeSelectionId,
        Guid LiveId,
        string? TituloTemporario,
        string? Descricao
    )
    {
        return new LiveBackstage(
            Guid.NewGuid(),
            TimeSelectionId,
            LiveId,
            TituloTemporario,
            Descricao
        );
    }

    public void Update(
        Guid TimeSelectionId,
        Guid LiveId,
        string? TituloTemporario,
        string? Descricao
    )
    {
        this.TimeSelectionId = TimeSelectionId;
        this.LiveId = LiveId;
        this.TituloTemporario = TituloTemporario;
        this.Descricao = Descricao;
    }
}
