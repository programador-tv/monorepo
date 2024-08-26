using Domain.Enumerables;
using Domain.Primitives;

namespace Domain.Entities;

public sealed class HelpResponse(
    Guid id,
    Guid timeSelectionId,
    Guid perfilId,
    string conteudo,
    DateTime createdAt,
    ResponseStatus responseStatus
) : Entity(id, createdAt)
{
    public Guid TimeSelectionId { get; private set; } = timeSelectionId;
    public Guid PerfilId { get; private set; } = perfilId;
    public string Conteudo { get; private set; } = conteudo;
    public ResponseStatus ResponseStatus { get; private set; } = responseStatus;

    public static HelpResponse Create(Guid timeSelecionId, Guid perfilId, string conteudo)
    {
        return new HelpResponse(
            Guid.NewGuid(),
            timeSelecionId,
            perfilId,
            conteudo,
            DateTime.Now,
            ResponseStatus.Posted
        );
    }

    public void DeleteResponse()
    {
        this.ResponseStatus = ResponseStatus.Deleted;
    }
}
