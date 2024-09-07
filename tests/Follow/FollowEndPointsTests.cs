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

public class FollowEndPointsTests
{
    private readonly Mock<IFollowBusinessLogic> mockLogic;

    public FollowEndPointsTests()
    {
        mockLogic = new Mock<IFollowBusinessLogic>();
    }

    [Fact]
    public async Task ToggleFollow_ShouldReturnResponse_WhenOk()
    {
        var guidFollowerId = Guid.NewGuid();
        var guidFollowingId = Guid.NewGuid();

        mockLogic
            .Setup(logic => logic.ToggleFollow(guidFollowerId, guidFollowingId))
            .ReturnsAsync(new ToggleFollowResponse(true));

        var result = await FollowEndPoints.ToggleFollow(
            mockLogic.Object,
            guidFollowerId,
            guidFollowingId
        );

        Assert.IsType<Ok<ToggleFollowResponse>>(result);
    }

    [Fact]
    public async Task ToggleFollow_ShouldReturnResponse_WhenException()
    {
        var guidFollowerId = Guid.NewGuid();
        var guidFollowingId = Guid.NewGuid();

        mockLogic
            .Setup(logic => logic.ToggleFollow(guidFollowerId, guidFollowingId))
            .ThrowsAsync(new Exception("Erro de teste"));

        var result = await FollowEndPoints.ToggleFollow(
            mockLogic.Object,
            guidFollowerId,
            guidFollowingId
        );

        Assert.IsType<BadRequest<string>>(result);
    }

    [Fact]
    public async Task GetFollowInformation_ShouldReturn_Ok()
    {
        var guidUserId = Guid.NewGuid();
        var expectedResponse = new FollowInformationResponse(2, 2);

        mockLogic
            .Setup(logic => logic.GetFollowInformation(guidUserId))
            .ReturnsAsync(expectedResponse);

        var result = await FollowEndPoints.GetFollowInformation(mockLogic.Object, guidUserId);

        Assert.IsType<Ok<FollowInformationResponse>>(result);
    }

    [Fact]
    public async Task GetFollowInformation_ShouldReturnResponse_WhenException()
    {
        var guidUserId = Guid.NewGuid();

        mockLogic
            .Setup(logic => logic.GetFollowInformation(guidUserId))
            .ThrowsAsync(new Exception("Erro de teste"));

        var result = await FollowEndPoints.GetFollowInformation(mockLogic.Object, guidUserId);

        Assert.IsType<BadRequest<string>>(result);
    }

    [Fact]
    public async Task IsFollowIng_ShouldReturnResponse_WhenOK()
    {
        var expectedFollowerId = Guid.NewGuid();
        var expectedfollowingId = Guid.NewGuid();
        var expectedResponse = new IsFollowingResponse(true);


        mockLogic
            .Setup(logic => logic.IsFollowing(expectedFollowerId, expectedfollowingId))
            .ReturnsAsync(expectedResponse);

        var result = await FollowEndPoints.IsFollowing(mockLogic.Object,expectedFollowerId, expectedfollowingId);

        Assert.IsType<Ok<IsFollowingResponse>>(result);
    }

    [Fact]
    public async Task IsFollowIng_ShouldReturnResponse_WhenException()
    {
        var expectedFollowerId = Guid.NewGuid();
        var expectedfollowingId = Guid.NewGuid();
        var expectedResponse = new Exception("teste de erro");

        mockLogic
            .Setup(logic => logic.IsFollowing(expectedFollowerId, expectedfollowingId))
            .ThrowsAsync(expectedResponse);

        var result = await FollowEndPoints.IsFollowing(mockLogic.Object, expectedFollowerId, expectedfollowingId);

        Assert.IsType<BadRequest<string>>(result);
    }
}
