using Domain.Enums;

namespace Domain.Models.ViewModels;

public sealed class TimeSelectionForCalendarSectionViewModel
{
    public Guid Id { get; set; }
    public string? Title { get; set; } = "";
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public StatusTimeSelection Status { get; set; }
    public EnumTipoTimeSelection Tipo { get; set; }
    public bool ActionNeeded { get; set; }
}
