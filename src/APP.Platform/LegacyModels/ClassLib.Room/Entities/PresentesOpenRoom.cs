namespace Domain.Entities
{
    public sealed class PresentesOpenRoom : _BaseEntity
    {
        public Guid PerfilId { get; set; }
        public bool EstaPresente { get; set; }
        public string? Nome { get; set; }
        public string? Foto { get; set; }
        public DateTime DataEntrou { get; set; }
        public DateTime UltimaAtualizacao { get; set; }
    }
}
