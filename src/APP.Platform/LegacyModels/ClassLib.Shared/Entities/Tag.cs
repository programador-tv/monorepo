namespace Domain.Entities;

public sealed class Tag : _BaseEntity
{
    public string? Titulo { get; set; }
    public string? LiveRelacao { get; set; }
    public string? RoomRelacao { get; set; }
    public string? FreeTimeRelacao { get; set; }
    public string? PreLiveRelacao { get; set; }
}
