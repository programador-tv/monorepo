using Application.Logic;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.EndPoints;

namespace tests;

public class JoinTimeEndPointsTests
{
    [Fact]
    public async Task UpdateOldJoinTimes_ReturnsOkResult()
    {
        // Mock of IJoinTimeBusinessLogic
        var mockLogic = new Mock<IJoinTimeBusinessLogic>();

        mockLogic.Setup(logic => logic.UpdateOldJoinTimes()).Returns(Task.CompletedTask);

        // Call the UpdateOldJoinTimes method and verify the result
        var result = await JoinTimeEndPoints.UpdateOldJoinTimes(mockLogic.Object);

        Assert.IsType<Ok>(result);
    }

    [Fact]
    public async Task UpdateOldJoinTimes_ReturnsBadRequest_OnException()
    {
        // Mock of IJoinTimeBusinessLogic
        var mockLogic = new Mock<IJoinTimeBusinessLogic>();

        mockLogic
            .Setup(logic => logic.UpdateOldJoinTimes())
            .ThrowsAsync(new Exception("Test error"));

        // Call the UpdateOldJoinTimes method and verify the result
        var result = await JoinTimeEndPoints.UpdateOldJoinTimes(mockLogic.Object);

        Assert.IsType<BadRequest<string>>(result);
    }
}
