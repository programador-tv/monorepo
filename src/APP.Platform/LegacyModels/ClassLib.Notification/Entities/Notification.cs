using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Entities
{
    public class Notification : _BaseEntity
    {
        public Guid DestinoPerfilId { get; set; }
        public Guid GeradorPerfilId { get; set; }
        public TipoNotificacao TipoNotificacao { get; set; }
        public bool Vizualizado { get; set; }
        public DateTime DataCriacao { get; set; }
        public string? Conteudo { get; set; }
        public string? ActionLink { get; set; }
        public string? SecundaryLink { get; set; }
    }
}
