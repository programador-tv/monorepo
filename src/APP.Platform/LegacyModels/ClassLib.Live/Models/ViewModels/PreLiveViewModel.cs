using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Domain.Models.ViewModels;

public sealed class PreLiveViewModel
{
    public string? Titulo { get; set; }
    public string? Descricao { get; set; }
    public IFormFile? Thumbnail { get; set; }
    public string? IsUsingObs { get; set; }
}
