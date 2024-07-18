using Domain.Enums;

namespace Domain.Models.ViewModels;

public class LiveViewModel
{
    public string? CodigoLive { get; set; }
    public DateTime DataCriacao { get; set; }
    public string? FormatedDuration { get; set; }
    public string? Titulo { get; set; }
    public string? Descricao { get; set; }
    public string? Thumbnail { get; set; }
    public string? NomeCriador { get; set; }
    public string? UserNameCriador { get; set; }
    public string? FotoCriador { get; set; }
    public int QuantidadeDeVisualizacoes { get; set; }
    public bool Visibility { get; set; }
    public bool LiveEstaAberta { get; set; }
    public StatusLive StatusLive { get; set; }
    public bool IsTimeSelection { get; set; }
    public string? UrlAlias { get; set; }
}
