namespace Domain.Entities;

public sealed class Perfil : _BaseEntity
{
    public string? Nome { get; set; }
    public string? Foto { get; set; }
    public string? Token { get; set; }
    public string? UserName { get; set; }
    public string? Linkedin { get; set; }
    public string? GitHub { get; set; }
    public string? Bio { get; set; }
    public string? Email { get; set; }
    public string? Descricao { get; set; }
    public ExperienceLevel Experiencia { get; set; }
}
