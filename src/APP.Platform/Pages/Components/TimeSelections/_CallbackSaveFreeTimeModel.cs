using Domain.Entities;
using Domain.Models.ViewModels;

public class _CallbackSaveFreeTimeModel
{
    public KeyValuePair<
        TimeSelection,
        List<JoinTimeViewModel>
    > TimeSelectionAndJoinTimes { get; set; } = new();
    public Dictionary<TimeSelection, List<Perfil>>? TimeSelectionsCheckedUsers { get; set; } =
        new();
}
