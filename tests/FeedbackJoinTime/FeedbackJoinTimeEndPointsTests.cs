using Application.Logic;
using Domain.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Presentation.EndPoints;

namespace tests;

public class FeedbackJoinTimeEndPointsTests
{
    private readonly Mock<IFeedbackJoinTimeBusinessLogic> mockLogic;

    public FeedbackJoinTimeEndPointsTests()
    {
        mockLogic = new Mock<IFeedbackJoinTimeBusinessLogic>();
    }

    [Fact]
    public async Task PostFeedbackJoinTime_ShouldReturnResult()
    {
        var entity = FeedbackJoinTime.Create(Guid.NewGuid(), DateTime.Now);
        mockLogic
            .Setup(logic => logic.CreateFeedbackJoinTime(It.IsAny<Guid>()))
            .ReturnsAsync(entity);
        var post = await FeedbackJoinTimeEndpoints.PostFeedbackJoinTime(
            mockLogic.Object,
            Guid.NewGuid()
        );

        mockLogic.Verify(logic => logic.CreateFeedbackJoinTime(It.IsAny<Guid>()), Times.Once);

        Assert.NotNull(post);
        Assert.IsType<Ok<FeedbackJoinTime>>(post);
    }

    [Fact]
    public async Task PostFeedbackJoinTime_ShouldReturnBadRequest()
    {
        mockLogic
            .Setup(logic => logic.CreateFeedbackJoinTime(It.IsAny<Guid>()))
            .ThrowsAsync(new Exception("Generic Error"));
        var post = await FeedbackJoinTimeEndpoints.PostFeedbackJoinTime(
            mockLogic.Object,
            Guid.NewGuid()
        );

        mockLogic.Verify(logic => logic.CreateFeedbackJoinTime(It.IsAny<Guid>()), Times.Once);

        Assert.NotNull(post);
        Assert.IsType<BadRequest<string>>(post);
    }
}
