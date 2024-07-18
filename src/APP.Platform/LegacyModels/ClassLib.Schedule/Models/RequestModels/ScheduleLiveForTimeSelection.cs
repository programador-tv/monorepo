using Microsoft.AspNetCore.Http;

namespace Domain.RequestModels;

public sealed class ScheduleLiveForTimeSelection
{
    public string? Descricao { get; set; }
    public IFormFile? Thumbnail { get; set; }
}
