using Application.Logic;
using Domain.Contracts;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Repositories;
using Moq;

namespace tests;

public class FeedbackJoinTimeBusinessLogicTests
{
    private readonly FeedbackJoinTimeBusinessLogic _businessLogic;
    private readonly Mock<IFeedbackJoinTimeRepository> _mockRepository;

    public FeedbackJoinTimeBusinessLogicTests()
    {
        _mockRepository = new Mock<IFeedbackJoinTimeRepository>();
        _businessLogic = new FeedbackJoinTimeBusinessLogic(_mockRepository.Object);
    }

    [Fact]
    public async Task CreateFeedbackJoinTime_ShouldReturnFeedbackJoinTime()
    {
        var mockEntity = FeedbackJoinTime.Create(Guid.NewGuid(), DateTime.Now);
        _mockRepository
            .Setup(repo => repo.AddFeedbackJoinTime(It.IsAny<FeedbackJoinTime>()))
            .ReturnsAsync(mockEntity);
        var create = await _businessLogic.CreateFeedbackJoinTime(Guid.NewGuid());

        _mockRepository.Verify(
            repo => repo.AddFeedbackJoinTime(It.IsAny<FeedbackJoinTime>()),
            Times.Once
        );
        Assert.NotNull(create);
        Assert.IsType<FeedbackJoinTime>(create);
    }
}
