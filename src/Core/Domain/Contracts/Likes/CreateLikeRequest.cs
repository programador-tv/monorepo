using Domain.Enumerables;
using Microsoft.AspNetCore.Http;

namespace Domain.Contracts;

public record CreateLikeRequest(Guid EntityId, Guid RelatedUserId);
