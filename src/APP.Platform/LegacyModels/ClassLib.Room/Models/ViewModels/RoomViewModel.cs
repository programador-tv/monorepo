using Domain.Entities;
using Domain.Enums;

namespace Domain.Models.ViewModels;

public sealed class RoomViewModel
{
    public string? CodigoSala { get; set; }
    public bool EstaAberto { get; set; }

    public string? NomeCriador { get; set; }
    public string? FotoCriador { get; set; }
    public List<Presentes>? Presentes { get; set; }
    public DateTime DataCriacao { get; set; }
    public List<string>? Tags { get; set; }
    public EnumTipoSalas TipoSala { get; set; }
    public string? Titulo { get; set; }
}
