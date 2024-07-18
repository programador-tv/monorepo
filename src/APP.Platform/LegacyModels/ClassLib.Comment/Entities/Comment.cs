namespace Domain.Entities
{
    public sealed class Comment : _BaseEntity
    {
        public Guid PerfilId { get; set; }
        public Guid LiveId { get; set; }
        public string? Content { get; set; }
        public DateTime DataCriacao { get; set; }
        public bool IsValid { get; set; }
    }
}
