using Domain.Contracts;
using Domain.Entities;
using Domain.Enumerables;

namespace tests;

public class LikeEntityTest
{
    [Fact]
    public void Create_GenerateTypeOfLike()
    {
        var liveId = Guid.NewGuid();
        var relatedUserId = Guid.NewGuid();

        var like = Like.Create(liveId, relatedUserId);

        Assert.IsType<Like>(like);
        Assert.Equal(liveId, like.EntityId);
        Assert.Equal(relatedUserId, like.RelatedUserId);
        Assert.True(like.IsLiked);
    }

    [Fact]
    public void DoLike_ShouldUpdateDislikeForDoLike()
    {
        var liveId = Guid.NewGuid();
        var relatedUserId = Guid.NewGuid();

        var like = Like.Create(liveId, relatedUserId);
        like.Dislike();

        Assert.False(like.IsLiked);

        like.DoLike();

        Assert.True(like.IsLiked);
    }
}
