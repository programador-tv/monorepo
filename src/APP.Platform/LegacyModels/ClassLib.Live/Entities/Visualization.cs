namespace Domain.Entities
{
    public sealed class Visualization : _BaseEntity
    {
        public Guid LiveId { get; set; }
        public Guid PerfilId { get; set; }
        public string? IPV4 { get; set; }
        public DateTime DataEntrou { get; set; }
    }
}
