using Domain.Entities;
using Domain.Enumerables;

namespace Domain.Models.ViewModels;

public sealed class NotificationViewModel
{
    public Guid DestinoPerfilId { get; set; }
    public Guid GeradorPerfilId { get; set; }
    public TipoNotificacao TipoNotificacao { get; set; }
    public bool Vizualizado { get; set; }
    public DateTime DataCriacao { get; set; }
    public string? Conteudo { get; set; }
    public string? ActionLink { get; set; }
    public string? SecundaryLink { get; set; }
    public Perfil? PerfilGerador { get; set; }
}
