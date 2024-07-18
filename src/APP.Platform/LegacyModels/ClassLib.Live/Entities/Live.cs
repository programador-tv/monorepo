using Domain.Enums;

namespace Domain.Entities
{
    public sealed class Live : _BaseEntity
    {
        public Guid PerfilId { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? UltimaAtualizacao { get; set; }
        public string? FormatedDuration { get; set; }
        public string? CodigoLive { get; set; }
        public string? RecordedUrl { get; set; }
        public string? StreamId { get; set; }
        public bool LiveEstaAberta { get; set; }
        public string? Titulo { get; set; }
        public string? Descricao { get; set; }
        public string? Thumbnail { get; set; }
        public bool Visibility { get; set; }
        public int TentativasDeObterUrl { get; set; }
        public StatusLive StatusLive { get; set; }
        public bool IsUsingObs { get; set; }
        public string? UrlAlias { get; set; }
    }
}
