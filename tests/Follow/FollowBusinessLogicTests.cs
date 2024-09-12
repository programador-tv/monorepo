using Application.Logic;
using Domain.Contracts;
using Domain.Entities;
using Domain.Enumerables;
using Domain.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Presentation.EndPoints;

namespace tests;

public class FollowBusinessLogicTests
{
    private readonly FollowBusinessLogic _businessLogic;
    private readonly Mock<IFollowRepository> _mockRepository;

    public FollowBusinessLogicTests()
    {
        _mockRepository = new Mock<IFollowRepository>();
        _businessLogic = new FollowBusinessLogic(_mockRepository.Object);
    }

    [Fact]
    public async Task ToggleFollow_ShouldReturnResponse_WhenNotExists()
    {
        var guidFollowerId = Guid.NewGuid();
        var guidFollowingId = Guid.NewGuid();

        var newFollow = Follow.Create(guidFollowerId, guidFollowingId);

        _mockRepository
            .Setup(repo => repo.GetByIdAsync(guidFollowerId, guidFollowingId))
            .Returns(Task.FromResult<Follow?>(null));

        _mockRepository.Setup(repo => repo.Create(newFollow));

        var result = await _businessLogic.ToggleFollow(guidFollowerId, guidFollowingId);

        // Assert
        _mockRepository.Verify(
            repo => repo.GetByIdAsync(guidFollowerId, guidFollowingId),
            Times.Once
        );

        _mockRepository.Verify(repo => repo.Create(It.IsAny<Follow>()), Times.Once);

        _mockRepository.Verify(repo => repo.Update(It.IsAny<Follow>()), Times.Never);

        Assert.NotNull(result);
        Assert.IsType<ToggleFollowResponse>(result);
        Assert.True(result.Active);
    }

    [Fact]
    public async Task ToggleFollow_ShouldReturnResponse_WhenUnfollow()
    {
        var guidFollowerId = Guid.NewGuid();
        var guidFollowingId = Guid.NewGuid();

        var newFollow = Follow.Create(guidFollowerId, guidFollowingId);

        _mockRepository
            .Setup(repo => repo.GetByIdAsync(guidFollowerId, guidFollowingId))
            .ReturnsAsync(newFollow);

        _mockRepository.Setup(repo => repo.Update(It.IsAny<Follow>()));

        var result = await _businessLogic.ToggleFollow(guidFollowerId, guidFollowingId);

        // Assert
        _mockRepository.Verify(
            repo => repo.GetByIdAsync(guidFollowerId, guidFollowingId),
            Times.Once
        );

        _mockRepository.Verify(repo => repo.Update(It.IsAny<Follow>()), Times.Once);

        _mockRepository.Verify(repo => repo.Create(It.IsAny<Follow>()), Times.Never);

        Assert.NotNull(result);
        Assert.IsType<ToggleFollowResponse>(result);
        Assert.False(result.Active);
    }

    [Fact]
    public async Task ToggleFollow_ShouldReturnResponse_WhenFollow()
    {
        var guidFollowerId = Guid.NewGuid();
        var guidFollowingId = Guid.NewGuid();

        var newFollow = Follow.Create(guidFollowerId, guidFollowingId);

        newFollow.UnfollowUser();

        _mockRepository
            .Setup(repo => repo.GetByIdAsync(guidFollowerId, guidFollowingId))
            .ReturnsAsync(newFollow);

        _mockRepository.Setup(repo => repo.Update(It.IsAny<Follow>()));

        var result = await _businessLogic.ToggleFollow(guidFollowerId, guidFollowingId);

        // Assert
        _mockRepository.Verify(
            repo => repo.GetByIdAsync(guidFollowerId, guidFollowingId),
            Times.Once
        );

        _mockRepository.Verify(repo => repo.Update(It.IsAny<Follow>()), Times.Once);

        _mockRepository.Verify(repo => repo.Create(It.IsAny<Follow>()), Times.Never);

        Assert.NotNull(result);
        Assert.IsType<ToggleFollowResponse>(result);
        Assert.True(result.Active);
    }

    [Fact]
    public async Task GetFollowInformation_WorksFine()
    {
        var guidUserId = Guid.NewGuid();

        var guidFollowerIdOne = Guid.NewGuid();
        var guidFollowerIdTwo = Guid.NewGuid();

        var guidFollowingIdOne = Guid.NewGuid();
        var guidFollowingIdTwo = Guid.NewGuid();

        var newFollowOne = Follow.Create(guidFollowerIdOne, guidUserId);
        var newFollowTwo = Follow.Create(guidFollowerIdTwo, guidUserId);

        var newFollowThree = Follow.Create(guidUserId, guidFollowingIdOne);
        var newFollowFour = Follow.Create(guidUserId, guidFollowingIdTwo);

        _mockRepository.Setup(repo => repo.GetFollowersAsync(guidUserId)).ReturnsAsync(2);

        _mockRepository.Setup(repo => repo.GetFollowingAsync(guidUserId)).ReturnsAsync(2);

        var result = await _businessLogic.GetFollowInformation(guidUserId);

        Assert.NotNull(result);
        Assert.IsType<FollowInformationResponse>(result);
        Assert.Equal(2, result.Followers);
        Assert.Equal(2, result.Following);
    }

    [Fact]
    public async Task IsFollow_ShouldReturnResponse_True()
    {
        var expectedFollowerId = Guid.NewGuid();
        var expectedfollowingId = Guid.NewGuid();
        var expectedResponse = new IsFollowingResponse(true);

        _mockRepository
            .Setup(repo => repo.IsFollowingAsync(expectedFollowerId, expectedfollowingId))
            .ReturnsAsync(expectedResponse);

        var result = await _businessLogic.IsFollowing(expectedFollowerId, expectedfollowingId);

        Assert.NotNull(result);
        Assert.IsType<IsFollowingResponse>(result);
        Assert.Equal(expectedResponse, result); 
    }
}
