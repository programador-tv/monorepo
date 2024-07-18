using Domain.Entities;

namespace Domain.Models.ViewModels;

public sealed class MentorFreeTime
{
    public Perfil? Perfils { get; set; }
    public List<TimeSelectionForMentorFreeTimeViewModel> TimeSelections { get; set; } = new();
}
