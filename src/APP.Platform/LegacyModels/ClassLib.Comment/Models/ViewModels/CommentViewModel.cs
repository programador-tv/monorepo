using Domain.Entities;

namespace Domain.Enums
{
    public sealed class CommentViewModel
    {
        public Guid Id { get; set; }
        public Perfil? Perfil { get; set; }
        public string? Content { get; set; }
        public DateTime DataCriacao { get; set; }
        public string? TempoQuePassou { get; set; }
    }
}
