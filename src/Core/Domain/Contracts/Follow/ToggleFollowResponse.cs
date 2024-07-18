using Domain.Enumerables;
using Microsoft.AspNetCore.Http;

namespace Domain.Contracts;

public record ToggleFollowResponse(bool Active);
