using Domain.Enums;

namespace Domain.Models.ViewModels;

public sealed class TimeSelectionViewModel
{
    public Guid Id { get; set; }
    public Guid? PerfilId { get; set; }
    public StatusTimeSelection Status { get; set; }
    public string? Title { get; set; } = "";
    public EnumTipoTimeSelection Tipo { get; set; }
    public List<string>? Tags { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string? Descricao { get; set; }
    public string? Link { get; set; }
    public bool ActionNeeded { get; set; }
}
