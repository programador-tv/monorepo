using Domain.Contracts;
using Domain.Entities;
using Domain.Enumerables;

namespace tests;

public class FollowlEntity
{
    [Fact]
    public void Create_GenereteTypeOfFollow()
    {
        var guidFollowerId = Guid.NewGuid();
        var guidFollowingId = Guid.NewGuid();

        var follow = Follow.Create(guidFollowerId, guidFollowingId);

        Assert.IsType<Follow>(follow);
        Assert.Equal(guidFollowerId, follow.FollowerId);
        Assert.Equal(guidFollowingId, follow.FollowingId);
        Assert.True(follow.Active);
    }

    [Fact]
    public void FollowUser_ShouldUpdateActiveForTrue()
    {
        var guidFollowerId = Guid.NewGuid();
        var guidFollowingId = Guid.NewGuid();

        var follow = Follow.Create(guidFollowerId, guidFollowingId);
        follow.UnfollowUser();

        Assert.False(follow.Active);

        follow.FollowUser();

        Assert.True(follow.Active);
    }
}
