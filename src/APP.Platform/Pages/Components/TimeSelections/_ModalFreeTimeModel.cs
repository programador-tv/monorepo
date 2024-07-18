using Domain.Entities;
using Domain.Models.ViewModels;

public class _ModalFreeTimeModel
{
    public Dictionary<
        TimeSelection,
        List<JoinTimeViewModel>
    >? TimeSelectionAndJoinTimes { get; set; } = new();
    public string TimeSelectionId { get; set; } = string.Empty;
}
