namespace Domain.Entities;

public sealed class Like : _BaseEntity
{
    public Guid EntityId { get; set; }
    public Guid RelatedUserId { get; set; }
    public bool IsLiked { get; set; }
}
