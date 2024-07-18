using Domain.Entities;
using Domain.Enums;

namespace Domain.Models.ViewModels;

public sealed class TimeSelectionForMentorFreeTimeViewModel
{
    public string TimeSelectionId { get; set; } = Guid.Empty.ToString();
    public string PerfilId { get; set; } = Guid.Empty.ToString();
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public List<Tag>? Tags { get; set; }
    public int CountInteressados { get; set; }
    public int CountInteressadosAceitos { get; set; }
    public int MaxParticipantes { get; set; }
    public string? Titulo { get; set; }
    public int Variacao { get; set; }
    public EnumTipoTimeSelection Type { get; set; } = EnumTipoTimeSelection.FreeTime;
}
