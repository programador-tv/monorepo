using Domain.Entities;
using Domain.Models.ViewModels;

namespace APP.Platform.Pages.Components.TimeSelections;

public class ModalFreeTimePanel
{
    public Dictionary<
        TimeSelection,
        List<JoinTimeViewModel>
    >? TimeSelectionAndJoinTimes { get; set; } = new();
}
