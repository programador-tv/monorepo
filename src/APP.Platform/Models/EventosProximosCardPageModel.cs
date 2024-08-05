namespace Models;

public sealed class EventosProximosCardPageModel : BaseCardPageModel
{
    public string FotoEnvolvido { get; set; } = string.Empty;
    public string NomeEnvolvido { get; set; } = string.Empty;
    public string TempoRestante { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}
