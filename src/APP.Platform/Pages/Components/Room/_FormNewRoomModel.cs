using Domain.Entities;
using Domain.Models.ViewModels;

public sealed class _FormNewRoomModel
{
    public Room? Room { get; set; }
    public List<string>? TagsSelected { get; set; }
    public Dictionary<string, List<string>>? RelatioTags { get; set; }
}
