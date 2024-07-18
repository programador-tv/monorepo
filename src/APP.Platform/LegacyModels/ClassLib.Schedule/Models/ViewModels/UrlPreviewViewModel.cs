using Domain.Entities;

namespace Domain.Models.ViewModels;

public class UrlPreviewViewModel
{
    public string? RelationId { get; set; }
    public string? PerfilId { get; set; }
    public string? UserName { get; set; }
    public string? UserNickName { get; set; }
    public string? UserPhoto { get; set; }
    public string? TempTitle { get; set; }
    public string? Preview { get; set; }
    public List<Tag> Tags { get; set; } = new();
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? TempoTotal { get; set; }
}
