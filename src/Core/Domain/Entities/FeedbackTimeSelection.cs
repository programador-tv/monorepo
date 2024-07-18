using Domain.Enums;

namespace Domain.Entities;

public sealed class FeedbackTimeSelection(
    Guid id,
    Guid timeSelectionId,
    DateTime? dataDeclaracao,
    DateTime? aceite,
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
    : Feedback(
        id,
        avaliadoCompareceu,
        avaliadorCompareceu,
        dataCancelamento,
        duracaoPrevista,
        duracaoPrevistaFormatada,
        estimativaSenioridadeAvaliado,
        estimativaSalarioAvaliado,
        conheciaAvaliadoPreviamente,
        satisfacaoExperiencia
    )
{
    public Guid TimeSelectionId { get; private set; } = timeSelectionId;
    public DateTime? DataDeclaracao { get; private set; } = dataDeclaracao;
    public DateTime? Aceite { get; private set; } = aceite;

    public static FeedbackTimeSelection Create(
        Guid timeSelectionId,
        DateTime? dataDeclaracao,
        DateTime? aceite,
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
        return new FeedbackTimeSelection(
            Guid.NewGuid(),
            timeSelectionId,
            dataDeclaracao,
            aceite,
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
