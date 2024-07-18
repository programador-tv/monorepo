using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Domain.Models.ViewModels;

public sealed class EditorLiveViewModel
{
    public string? Titulo { get; set; }
    public string? Descricao { get; set; }
    public bool Visibility { get; set; }
    public IFormFile? Thumbnail { get; set; }
}
