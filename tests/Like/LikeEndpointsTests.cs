using Application.Logic;
using Domain.Contracts;
using Domain.Entities;
using Domain.Enumerables;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.EndPoints;

namespace tests;

public class LikeEndPointsTests
{
    private readonly Mock<ILikeBusinessLogic> mockLogic;

    public LikeEndPointsTests()
    {
        mockLogic = new Mock<ILikeBusinessLogic>();
    }

    [Fact]
    public async Task GetLikesLiveById_ShouldReturnResponse_WhenOk()
    {
        var liveId = Guid.NewGuid();

        var relatedUserIdOne = Guid.NewGuid();
        var relatedUserIdTwo = Guid.NewGuid();

        var likes = new List<Like>
        {
            Like.Create(liveId, relatedUserIdOne),
            Like.Create(liveId, relatedUserIdTwo),
        };

        mockLogic.Setup(logic => logic.GetLikesByLiveId(liveId)).ReturnsAsync(likes);

        var result = await LikeEndPoints.GetLikesLiveById(mockLogic.Object, liveId);

        Assert.IsType<Ok<List<Like>>>(result);
    }

    [Fact]
    public async Task GetLikesLiveById_ShouldReturnResponse_WhenException()
    {
        var liveId = Guid.NewGuid();

        mockLogic
            .Setup(logic => logic.GetLikesByLiveId(liveId))
            .ThrowsAsync(new Exception("Erro de teste"));

        var result = await LikeEndPoints.GetLikesLiveById(mockLogic.Object, liveId);

        Assert.IsType<BadRequest<string>>(result);
    }
}
