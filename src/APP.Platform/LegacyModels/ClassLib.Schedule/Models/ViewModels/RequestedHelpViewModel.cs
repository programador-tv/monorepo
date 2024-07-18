using Domain.Entities;

namespace Domain.Models.ViewModels;

public class RequestedHelpViewModel
{
    public Perfil? Perfils { get; set; }
    public List<TimeSelectionForRequestedHelpViewModel> TimeSelections { get; set; } = [];
}
