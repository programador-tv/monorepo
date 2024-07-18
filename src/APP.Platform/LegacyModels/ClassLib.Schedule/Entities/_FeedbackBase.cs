using Domain.Enums;

namespace Domain.Entities
{
    public class _FeedbackBase : _BaseEntity
    {
        public bool AvaliadoCompareceu { get; set; }
        public bool AvaliadorCompareceu { get; set; }
        public DateTime? DataCancelamento { get; set; }
        public TimeSpan? DuracaoPrevista { get; set; }
        public string? DuracaoPrevistaFormatada { get; set; }
        public EstimativaSenioridade? EstimativaSenioridadeAvaliado { get; set; }
        public decimal? EstimativaSalarioAvaliado { get; set; }
        public bool? ConheciaAvaliadoPreviamente { get; set; }
        public SatisfacaoExperiencia? SatisfacaoExperiencia { get; set; }
    }
}
