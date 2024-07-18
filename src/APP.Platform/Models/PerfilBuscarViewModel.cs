using Domain.Entities;

namespace Domain.Models.ViewModels;

public sealed class PerfilBuscarViewModel
{
    public Guid Id { get; set; }
    public string? Nome { get; set; }

    public string? UserName { get; set; }
    public string? Foto { get; set; }

    public string? Bio { get; set; }

    public int Followers { get; set; }

    public bool isFollowing { get; set; }
}
