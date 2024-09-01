using Domain.Enumerables;
using Microsoft.AspNetCore.Http;

namespace Domain.Contracts;

public record CreateOrUpdatePerfilRequest(
    string Nome,
    string Token,
    string UserName,
    string Linkedin,
    string GitHub,
    string Bio,
    string Email,
    string Descricao,
    ExperienceLevel Experiencia
);
