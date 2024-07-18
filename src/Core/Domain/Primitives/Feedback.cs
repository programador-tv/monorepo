using Domain.Enums;
using Domain.Primitives;

namespace Domain.Entities
{
    public class Feedback(
        Guid id,
        bool avaliadoCompareceu,
        bool avaliadorCompareceu,
        DateTime? dataCancelamento,
        TimeSpan? duracaoPrevista,
        string? duracaoPrevistaFormatada,
        EstimativaSenioridade? estimativaSenioridadeAvaliado,
        decimal? estimativaSalarioAvaliado,
        bool? conheciaAvaliadoPreviamente,
        SatisfacaoExperiencia? satisfacaoExperiencia
    ) : Entity(id)
    {
        public bool AvaliadoCompareceu { get; private set; } = avaliadoCompareceu;
        public bool AvaliadorCompareceu { get; private set; } = avaliadorCompareceu;
        public DateTime? DataCancelamento { get; private set; } = dataCancelamento;
        public TimeSpan? DuracaoPrevista { get; private set; } = duracaoPrevista;
        public string? DuracaoPrevistaFormatada { get; private set; } = duracaoPrevistaFormatada;
        public EstimativaSenioridade? EstimativaSenioridadeAvaliado { get; private set; } =
            estimativaSenioridadeAvaliado;
        public decimal? EstimativaSalarioAvaliado { get; private set; } = estimativaSalarioAvaliado;
        public bool? ConheciaAvaliadoPreviamente { get; private set; } =
            conheciaAvaliadoPreviamente;
        public SatisfacaoExperiencia? SatisfacaoExperiencia { get; private set; } =
            satisfacaoExperiencia;

        public static Feedback Create(
            bool avaliadoCompareceu,
            bool avaliadorCompareceu,
            DateTime? dataCancelamento,
            TimeSpan? duracaoPrevista,
            string? duracaoPrevistaFormatada,
            EstimativaSenioridade? estimativaSenioridadeAvaliado,
            decimal? estimativaSalarioAvaliado,
            bool? conheciaAvaliadoPreviamente,
            SatisfacaoExperiencia? satisfacaoExperiencia
        )
        {
            return new Feedback(
                Guid.NewGuid(),
                avaliadoCompareceu,
                avaliadorCompareceu,
                dataCancelamento,
                duracaoPrevista,
                duracaoPrevistaFormatada,
                estimativaSenioridadeAvaliado,
                estimativaSalarioAvaliado,
                conheciaAvaliadoPreviamente,
                satisfacaoExperiencia
            );
        }
    }
}
