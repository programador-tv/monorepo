using Microsoft.AspNetCore.Http;

namespace Domain.Contracts;

public record CreateHelpBackstageRequest(Guid TimeSelectionId, string Description);
