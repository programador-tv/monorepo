using Domain.Contracts;
using Domain.Primitives;

namespace Domain.Entities;

public sealed class HelpBackstage(
    Guid id,
    Guid TimeSelectionId,
    string? Descricao,
    string? ImagePath
) : Entity(id)
{
    public Guid TimeSelectionId { get; private set; } = TimeSelectionId;
    public string? Descricao { get; private set; } = Descricao;
    public string? ImagePath { get; set; } = ImagePath;

    public static HelpBackstage Create(CreateHelpBackstageRequest backstage)
    {
        return new(Guid.NewGuid(), backstage.TimeSelectionId, backstage.Description, null);
    }

    public void AddImagePath(string path)
    {
        ImagePath = path;
    }
}
