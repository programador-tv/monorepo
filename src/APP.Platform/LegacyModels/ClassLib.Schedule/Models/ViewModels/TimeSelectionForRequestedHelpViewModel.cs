using Domain.Entities;
using Domain.Enums;

namespace Domain.Models.ViewModels;

public class TimeSelectionForRequestedHelpViewModel
{
    public string TimeSelectionId { get; set; } = Guid.Empty.ToString();
    public string PerfilId { get; set; } = Guid.Empty.ToString();
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public List<Tag>? Tags { get; set; }
    public string? Description { get; set; }
    public string? Title { get; set; }
    public string? ImagePath { get; set; }
    public int Variation { get; set; }
    public EnumTipoTimeSelection Type { get; set; } = EnumTipoTimeSelection.RequestHelp;
    public List<HelpResponse> HelpResponses { get; set; } = new List<HelpResponse>();
}
