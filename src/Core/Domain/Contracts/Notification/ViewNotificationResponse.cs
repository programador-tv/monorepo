using Domain.Enumerables;

namespace Domain.Contracts;

public record NotificationItemResponse(
    Guid DestinoPerfilId,
    Guid GeradorPerfilId,
    TipoNotificacao TipoNotificacao,
    bool Vizualizado,
    DateTime DataCriacao,
    string? Conteudo,
    string? ActionLink,
    string? SecundaryLink
);
