using Domain.Entities;

namespace Domain.Contracts;

public record HelpResponseWithProfileData(
    HelpResponse helpResponse,
    string? profileUserName,
    string? profileNome,
    string? profileFoto
);
