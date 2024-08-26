using Domain.Contracts;
using Domain.Primitives;

namespace Domain.Entities;

public sealed class HelpBackstage(
    Guid id,
    Guid TimeSelectionId,
    string? Descricao,
    string? ImagePath,
    DateTime createdAt
) : Entity(id, createdAt)
{
    public Guid TimeSelectionId { get; private set; } = TimeSelectionId;
    public string? Descricao { get; private set; } = Descricao;
    public string? ImagePath { get; set; } = ImagePath;

    public static HelpBackstage Create(CreateHelpBackstageRequest backstage)
    {
        return new(Guid.NewGuid(), backstage.TimeSelectionId, backstage.Description, null, DateTime.Now);
    }

    public void AddImagePath(string path)
    {
        ImagePath = path;
    }
}
