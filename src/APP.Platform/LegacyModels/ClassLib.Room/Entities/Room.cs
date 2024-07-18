using Domain.Enums;

namespace Domain.Entities
{
    public sealed class Room : _BaseEntity
    {
        public Guid PerfilId { get; set; }
        public string? CodigoSala { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? UltimaAtualizacao { get; set; }
        public bool EstaAberto { get; set; }
        public EnumTipoSalas TipoSala { get; set; }
        public string? Titulo { get; set; }
        public bool Privado { get; set; }
    }
}
