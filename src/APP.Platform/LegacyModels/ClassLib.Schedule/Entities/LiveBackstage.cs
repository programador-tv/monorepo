namespace Domain.Entities
{
    public sealed class LiveBackstage : _BaseEntity
    {
        public Guid TimeSelectionId { get; set; }
        public Guid LiveId { get; set; }
        public string? TituloTemporario { get; set; }
        public string? Descricao { get; set; }
    }
}
