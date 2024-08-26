using Domain.Primitives;

namespace Domain.Entities;

public sealed class Follow(Guid id, Guid followerId, Guid followingId, bool active, DateTime createdAt) : Entity(id, createdAt)
{
    public Guid FollowerId { get; private set; } = followerId;
    public Guid FollowingId { get; private set; } = followingId;
    public Boolean Active { get; private set; } = active;

    public static Follow Create(Guid followerId, Guid followingId)
    {
        return new Follow(Guid.NewGuid(), followerId, followingId, true, DateTime.Now);
    }

    public void FollowUser()
    {
        Active = true;
    }

    public void UnfollowUser()
    {
        Active = false;
    }
}
