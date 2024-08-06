using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.Models.ViewModels;

public sealed class CalendarRenderObjectsViewModel
{
    public List<TimeSelectionForCalendarSectionViewModel>? MyTimeSelection { get; set; }
    public List<TimeSelectionForCalendarSectionViewModel>? AttachedTimeSelection { get; set; }
    public List<BadFinishedTimeSelectionForCalendarSectionViewModel>? BadFinished { get; set; }
    public string? Modals { get; set; }
    public string? TimeSelectionPanelModals { get; set; }
    public string? LivesModalsPanel { get; set; }
    public string? JoinTimeModalsPanel { get; set; }
    public string? RequestHelpModalsPanel { get; set; }
    public string? SolvedHelpModalsPanel { get; set; }
}
