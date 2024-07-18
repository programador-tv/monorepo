namespace Domain.Models
{
    public sealed class CreateChatMessageRequest
    {
        public Guid Id { get; set; }
        public Guid PerfilId { get; set; }
        public Guid LiveId { get; set; }
        public string? Content { get; set; }
        public DateTime DataCriacao { get; set; }

        public string? Foto { get; set; }
        public string? Nome { get; set; }
        public string? Data { get; set; }
    }
}
