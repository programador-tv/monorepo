using System.ComponentModel.DataAnnotations;
using Domain.Contracts;
using Domain.Enumerables;
using Domain.Primitives;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[Index(nameof(Titulo), nameof(Visibility))]
[Index(nameof(Descricao), nameof(Visibility))]
[Index(nameof(LiveEstaAberta))]
public class Live(
    Guid id,
    Guid perfilId,
    DateTime createdAt,
    DateTime? ultimaAtualizacao,
    string? formatedDuration,
    string? codigoLive,
    string? recordedUrl,
    string? streamId,
    bool liveEstaAberta,
    string? titulo,
    string? descricao,
    string? thumbnail,
    bool visibility,
    int tentativasDeObterUrl,
    StatusLive statusLive,
    bool isUsingObs,
    string? urlAlias
) : Entity(id, createdAt)
{
    public Guid PerfilId { get; private set; } = perfilId;
    public DateTime? UltimaAtualizacao { get; private set; } = ultimaAtualizacao;
    public string? FormatedDuration { get; private set; } = formatedDuration;
    public string? CodigoLive { get; private set; } = codigoLive;
    public string? RecordedUrl { get; private set; } = recordedUrl;
    public string? StreamId { get; private set; } = streamId;
    public bool LiveEstaAberta { get; private set; } = liveEstaAberta;
    public string? Titulo { get; private set; } = titulo;

    [MaxLength(2000)]
    public string? Descricao { get; private set; } = descricao;

    public string? Thumbnail { get; private set; } = thumbnail;
    public bool Visibility { get; private set; } = visibility;
    public int TentativasDeObterUrl { get; private set; } = tentativasDeObterUrl;
    public StatusLive StatusLive { get; private set; } = statusLive;
    public bool IsUsingObs { get; private set; } = isUsingObs;
    public string? UrlAlias { get; private set; } = urlAlias;

    public static Live Create(CreateLiveRequest createLiveRequest)
    {
        return new Live(
            Guid.NewGuid(),
            perfilId: createLiveRequest.PerfilId,
            DateTime.Now,
            DateTime.Now,
            string.Empty,
            string.Empty,
            string.Empty,
            streamId: createLiveRequest.StreamId,
            false,
            titulo: createLiveRequest.Titulo,
            descricao: createLiveRequest.Descricao,
            thumbnail: createLiveRequest.Thumbnail,
            false,
            0,
            StatusLive.Preparando,
            isUsingObs: createLiveRequest.IsUsingObs,
            urlAlias: createLiveRequest.UrlAlias
        );
    }

    public void AtualizaThumbnail(string imageBase64)
    {
        Thumbnail = imageBase64;
    }

    public void Inicia()
    {
        LiveEstaAberta = true;
        StatusLive = StatusLive.Iniciada;
    }

    public void Encerra()
    {
        LiveEstaAberta = false;
        StatusLive = StatusLive.Encerrada;
    }

    public void Finaliza()
    {
        LiveEstaAberta = false;
        StatusLive = StatusLive.Finalizada;
    }

    public void AtualizaDuracao(string formatedDuration)
    {
        FormatedDuration = formatedDuration;
    }

    public void MantemAberta()
    {
        if (!LiveEstaAberta)
        {
            LiveEstaAberta = true;
        }
        if (StatusLive != StatusLive.Iniciada)
        {
            StatusLive = StatusLive.Iniciada;
        }
        UltimaAtualizacao = DateTime.Now;
    }
}
