using Domain.Entities;
using Domain.Enums;

namespace Domain.Models.ViewModels;

public sealed class JoinTimeViewModel
{
    public Perfil? Perfil { get; set; }
    public Guid JoinTimeId { get; set; }
    public Guid? TimeSelectionId { get; set; }
    public StatusJoinTime StatusJoinTime { get; set; }
}
