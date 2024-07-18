using Domain.Enumerables;
using Microsoft.AspNetCore.Http;

namespace Domain.Contracts;

public record UpdatePerfilRequest(
    Guid Id,
    string Nome,
    string UserName,
    string Linkedin,
    string GitHub,
    string Bio,
    string Descricao,
    ExperienceLevel Experiencia
);
