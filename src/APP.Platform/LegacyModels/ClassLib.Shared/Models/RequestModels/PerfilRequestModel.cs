using System.ComponentModel.DataAnnotations;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Domain.Models.Request;

public sealed class PerfilViewModel
{
    public Guid Id { get; set; }
    public string? Nome { get; set; }
    public IFormFile? Foto { get; set; }
    public string? UserName { get; set; }
    public string? Linkedin { get; set; }
    public string? GitHub { get; set; }
    public string? Bio { get; set; }
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "Selecione o nível de experiência")]
    public ExperienceLevel Experiencia { get; set; }
}
