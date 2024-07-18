using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Domain.RequestModels;

public sealed class ScheduleTimeSelectionRequestModel
{
    public string? Titulo { get; set; }
    public string? Descricao { get; set; }
    public IFormFile? ImageFile { get; set; }
    public Variacao Variacao { get; set; }
    public EnumTipoTimeSelection Tipo { get; set; }
    public StatusTimeSelection Status { get; set; } = StatusTimeSelection.Pendente;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public List<string> TagsSelected { get; set; } = new();
}
