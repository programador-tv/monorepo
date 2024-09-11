using Domain.Enums;
using Domain.Primitives;

namespace Domain.Entities;

public sealed class FeedbackJoinTime(
    Guid id,
    Guid joinTimeId,
    DateTime? dataTentativaMarcacao,
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
    public Guid JoinTimeId { get; private set; } = joinTimeId;
    public DateTime? DataTentativaMarcacao { get; private set; } = dataTentativaMarcacao;

    public static FeedbackJoinTime Create(
        Guid joinTimeId,
        DateTime? dataTentativaMarcacao,
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
        return new FeedbackJoinTime(
            Guid.NewGuid(),
            joinTimeId,
            dataTentativaMarcacao,
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

    public static FeedbackJoinTime Create(Guid joinTimeId, DateTime? dataTentativaMarcacao)
    {
        return new FeedbackJoinTime(
            Guid.NewGuid(),
            joinTimeId,
            dataTentativaMarcacao,
            false,
            false,
            null,
            null,
            null,
            null,
            null,
            null,
            null
        );
    }
}
