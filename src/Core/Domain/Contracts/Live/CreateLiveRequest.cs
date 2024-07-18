namespace Domain.Contracts;

public record CreateLiveRequest(
    Guid PerfilId,
    string? Titulo,
    string? Descricao,
    string? Thumbnail,
    bool IsUsingObs,
    string StreamId,
    string? UrlAlias
);
