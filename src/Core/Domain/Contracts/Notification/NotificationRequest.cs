using Domain.Enumerables;
using Microsoft.AspNetCore.Http;

namespace Domain.Contracts;

public record CreateNotificationRequest(
    Guid DestinoPerfilId,
    Guid GeradorPerfilId,
    TipoNotificacao TipoNotificacao,
    string? Conteudo,
    string? ActionLink,
    string? SecundaryLink
);
