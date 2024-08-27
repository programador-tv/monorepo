using Application.Logic;
using Domain.Contracts;
using Domain.Entities;
using Domain.Enumerables;
using Domain.Repositories;
using Moq;

namespace tests;

public class LikeBusinessLogicTests
{
    private readonly LikeBusinessLogic _businessLogic;
    private readonly Mock<ILikeRepository> _mockRepository;

    public LikeBusinessLogicTests()
    {
        _mockRepository = new Mock<ILikeRepository>();
        _businessLogic = new LikeBusinessLogic(_mockRepository.Object);
    }

    [Fact]
    public async Task GetLikesByLiveId_ShouldReturnListOfLikes()
    {
        var liveId = Guid.NewGuid();

        var relatedUserIdOne = Guid.NewGuid();
        var relatedUserIdTwo = Guid.NewGuid();

        var likes = new List<Like>
        {
            Like.Create(liveId, relatedUserIdOne),
            Like.Create(liveId, relatedUserIdTwo),
        };

        _mockRepository.Setup(repo => repo.GetAllLikesByLiveId(liveId)).ReturnsAsync(likes);

        var result = await _businessLogic.GetLikesByLiveId(liveId);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        foreach (var like in result)
        {
            Assert.Equal(liveId, like.EntityId);
            Assert.True(
                like.RelatedUserId == relatedUserIdOne || like.RelatedUserId == relatedUserIdTwo
            );
        }
    }

    [Fact]
    public async Task GetLikesByLiveId_ShouldReturnAnEmptyListOfLikes()
    {
        var liveId = Guid.NewGuid();

        _mockRepository
            .Setup(repo => repo.GetAllLikesByLiveId(liveId))
            .ReturnsAsync(new List<Like>());

        var result = await _businessLogic.GetLikesByLiveId(liveId);

        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
