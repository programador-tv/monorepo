using Domain.Enumerables;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Domain.Contracts;

public record CreateOrUpdatePerfilResponse(StatusCreateOrUpdatePerfil status, Guid Id);
