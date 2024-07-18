using Domain.Primitives;

namespace Domain.Entities;

public sealed class Like(Guid id, Guid entityId, Guid relatedUserId, bool isLiked) : Entity(id)
{
    public Guid EntityId { get; private set; } = entityId;
    public Guid RelatedUserId { get; private set; } = relatedUserId;
    public bool IsLiked { get; private set; } = isLiked;

    public static Like Create(Guid entityId, Guid relatedUserId)
    {
        return new Like(Guid.NewGuid(), entityId, relatedUserId, true);
    }

    public void DoLike()
    {
        IsLiked = true;
    }

    public void Dislike()
    {
        IsLiked = false;
    }
}
